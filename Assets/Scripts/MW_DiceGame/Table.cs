using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MW_DiceGame {

	public class Table : NetworkBehaviour {

		public enum GameState {
			Null,
			Preparation,
			Bidding,
			EvaluationPhase,
			GameOver
		}

		public delegate void ControlbarButtonsDelegate ();

		public delegate void BidButtonsDelegate (Bid newBid);

		public delegate void DiceHandlingDelegate ();

		[SyncEvent]
		public event ControlbarButtonsDelegate EventUnlockControls;
		[SyncEvent]
		public event ControlbarButtonsDelegate EventLockControls;
		[SyncEvent]
		public event BidButtonsDelegate EventOnBidChanged;
		[SyncEvent]
		public event DiceHandlingDelegate EventLookUpAllDices;


		[SyncVar (hook = "OnGameStateChange")]
		public GameState theGameState;

		//[SyncVar (hook = "OnBidChange")]
		[SyncVar]
		public Bid currentBid;

		public static Table singleton;

		[HideInInspector] public Transform players;
		IState gameState;
		int playerIndex = 0;
		[HideInInspector] public IDictionary<DieFaces, int> dieFaceMap;
		int clientsReadyCount = 0;

		[HideInInspector]
		public Preparation preparation;
		[HideInInspector]
		public Bidding bidding;
		[HideInInspector]
		public EvaluationPhase evaluationPhase;
		[HideInInspector]
		public GameOver gameOver;

		GamePlayer[] enabledPlayerCams;

		// ----- Callbacks --------------------------------------------------------------------------------------------------------------------------

		void Awake () {
			if (singleton == null) {
				singleton = this;
			} else if (singleton != this) {
				Destroy (gameObject);
			}

			dieFaceMap = new Dictionary<DieFaces, int> (Bid.maxBidDieFaceValue);

			this.preparation = new Preparation (this);
			this.bidding = new Bidding (this);
			this.evaluationPhase = new EvaluationPhase (this);
			this.gameOver = new GameOver (this);
		}

		public override void OnStartServer () {
			base.OnStartServer ();
			Debug.Log ("OnStartServer");

			NetworkServer.RegisterHandler (ActionMsg.ClientReady, OnClientReady);
			NetworkServer.RegisterHandler (ActionMsg.EnterBid, OnEnterBid);
			NetworkServer.RegisterHandler (ActionMsg.CallOutBluff, OnCallOutBluff);
			NetworkServer.RegisterHandler (ActionMsg.DeclareBidSpotOn, OnDeclareBidSpotOn);
		}

		public override void OnStartClient () {
			base.OnStartClient ();
			Debug.Log ("OnStartClient");

			/*if (isServer) {
				return;
			}*/

			InitPlayers ();
			//this.currentBid = currentBid;
			//if (EventOnBidChanged != null) {
			//EventOnBidChanged (BidAlreadyExists ());
			//}
		}

		void Start () {
			Debug.Log ("Start");
			//if (isServer) {
			//SetGameState (new Preparation (this));
			//}

			/*for (int i = 0; i < players.childCount; i++) {
				var player = players.GetChild (i);
				bool ready = player.GetComponent<SpawnManager> ().ready;
				if (!ready) {
					return;
				}
			}*/
			if (isServer) {
				Invoke ("StartGame", 3f);
			}
		}





		// ----- Hooks --------------------------------------------------------------------------------------------------------------------------

		void OnGameStateChange (GameState newState) {
			//Debug.Log ("Setze GameState auf " + newState);
			this.theGameState = newState;

			/*if (newState.Equals (GameState.Preparation) || newState.Equals (GameState.GameOver)) {
				return;
			}

			var gamePlayer = GetLocalPlayer ();
			DiceCup diceCup = gamePlayer.GetComponent<DiceCup> ();

			if (newState.Equals (GameState.Bidding)) {
				
				diceCup.HideDices ();
				SendUnlockControlsEvent ();

			} else {
				diceCup.HideDices ();
				SendLockControlsEvent ();
			}*/
		}

		/*GamePlayer GetLocalPlayer () {
			for (int i = 0; i < players.childCount; i++) {
				var gamePlayer = players.GetChild (i).GetComponent<GamePlayer> ();
				if (gamePlayer.isLocalPlayer) {
					return gamePlayer;
				}
			}

			return null;
		}*/

		[Server]
		public void SendUnlockControlsEvent () {
			Debug.Log ("Aktiviere Look und Hide Buttons");
			if (EventUnlockControls != null) {
				EventUnlockControls ();
			}
		}

		[Server]
		public void SendLockControlsEvent () {
			Debug.LogWarning ("Deaktiviere alle Buttons");
			if (EventLockControls != null) {
				EventLockControls ();
			}
		}

		/*void OnBidChange (Bid bid) {
			this.currentBid = bid;
				
			if (isServer) {
				ChangeBidOnClients (bid);
			}
		}*/

		[Server]
		public void ChangeBidOnClients (Bid bid) {
			Debug.LogWarning ("Informiere Clients, dass sich das Gebot geändert hat.");
			if (EventOnBidChanged != null) {
				EventOnBidChanged (bid);
			}
		}





		// ----- Network Handler ----------------------------------------------------------------------------------------------------------------

		[Server]
		public void OnClientReady (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();

			//CheckIfClientsReady (msg);
		}

		/*void CheckIfClientsReady (ActionMessage msg) {
			for (int i = 0; i < players.childCount; i++) {
				var player = players.GetChild (i);
				GamePlayer gamePlayer = player.GetComponent<GamePlayer> ();
				int connectionId = gamePlayer.connectionToClient.connectionId;

				if (connectionId == msg.connectionId) {
					this.clientsReadyCount++;
					break;
				}
			}

			StartGameIfAllClientsAreReady ();
		}

		public void ServerIsReady () {
			this.clientsReadyCount++;

			StartGameIfAllClientsAreReady ();
		}

		void StartGameIfAllClientsAreReady () {
			if (this.clientsReadyCount == players.childCount) {
				StartGame ();
			}
		}*/

		[Server]
		void OnEnterBid (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server erhält Anfrage für ein neues Gebot: " + msg.bid);

			if (MayAct (msg)) {
				gameState.SetActionStrategy (new EnterBid (msg.bid));
				Execute ();
				NextPlayer ();
			}
		}

		[Server]
		void OnCallOutBluff (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server erhält Anfrage für eine Bluff-Vermutung.");

			if (MayAct (msg)) {
				if (currentBid.Exists ()) {
					
					EnterEvaluationPhase ();
					gameState.SetActionStrategy (new CallOutBluff ());
					//Execute (new CallOutBluff ());
				}
			}
		}

		[Server]
		void OnDeclareBidSpotOn (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server erhält Anfrage für ein Verdacht auf ein präzises Gebot.");

			if (MayAct (msg)) {
				if (currentBid.Exists ()) {
					
					EnterEvaluationPhase ();
					gameState.SetActionStrategy (new DeclareBidSpotOn ());
					//Execute (new DeclareBidSpotOn ());
				}
			}
		}





		// ----- GameStates ---------------------------------------------------------------------------------------------------------------------

		[Server]
		public IState GetGameState () {
			return this.gameState;
		}

		[Server]
		public void SetGameState (IState gameState) {
			Debug.Log ("Setze GameState auf " + gameState);

			if (this.gameState != null) {
				this.gameState.OnExit ();
			}

			this.gameState = gameState;

			string stateName = gameState.GetType ().ToString ();
			this.theGameState = (GameState)Enum.Parse (typeof(GameState), stateName);

			this.gameState.OnEnter ();
		}

		[Server]
		public void Execute () {
			this.gameState.Execute ();
		}

		[Server]
		public void StartGame () {
			Debug.LogWarning ("StartGame");
			SetGameState (preparation);
			this.gameState.StartGame ();
		}

		[Server]
		public void NextPlayer () {
			Debug.LogWarning ("NextPlayer");
			this.gameState.NextPlayer ();
		}

		[Server]
		public void EnterEvaluationPhase () {
			this.gameState.EnterEvaluationPhase ();
		}

		[Server]
		public void EnterBidding () {
			this.gameState.EnterBidding ();
		}

		[Server]
		public void LeaveGame () {
			this.gameState.LeaveGame ();
		}



		public void ExecuteState () {
			if (isServer) {
				Execute ();
			}
		}

		public void FinishRound () {
			if (isServer) {
				EnterBidding ();
				//NextPlayer ();
			}
		}





		// ----- Player -------------------------------------------------------------------------------------------------------------------------

		void InitPlayers () {
			//Debug.Log ("InitPlayers");
			players = GameObject.Find ("Player").transform;
		}

		[Server]
		public Transform GetCurrentPlayer () {
			if (players.childCount > playerIndex) {
				return players.GetChild (playerIndex);
			}

			return null;
		}

		[Server]
		public Transform GetLastPlayer () {
			int lastPlayerIndex = playerIndex - 1;
			if (lastPlayerIndex < 0) {
				lastPlayerIndex = players.childCount - 1;
			}
			return players.GetChild (lastPlayerIndex);
		}

		[Server]
		public void NextPlayerHisTurn () {
			FinishTurnForPlayer ();
			UpdatePlayerIndex ();
			BeginTurnForNextPlayer ();
		}

		[Server]
		void FinishTurnForPlayer () {
			Transform player = GetCurrentPlayer ();
			var gamePlayer = player.GetComponent<GamePlayer> ();
			Debug.Log ("FinishTurnForPlayer " + gamePlayer.playerName);
			gamePlayer.CmdItIsYourTurn (false);
		}

		[Server]
		void UpdatePlayerIndex () {
			Debug.Log ("UpdatePlayerIndex");
			if (playerIndex + 1 < players.childCount) {
				playerIndex++;
			} else {
				playerIndex = 0;
			}
		}

		[Server]
		void BeginTurnForNextPlayer () {
			var player = GetCurrentPlayer ();
			var gamePlayer = player.GetComponent<GamePlayer> ();
			Debug.Log ("BeginTurnForNextPlayer " + gamePlayer.playerName);
			gamePlayer.CmdItIsYourTurn (true);
		}





		// ----- Bid ----------------------------------------------------------------------------------------------------------------------------

		[Server]
		public void ResetBid () {
			this.currentBid = new Bid (DieFaces.Null, 0);
		}





		// ----- Dices --------------------------------------------------------------------------------------------------------------------------

		[ClientRpc]
		public void RpcLookUpAllDices () {
			for (int i = 0; i < players.childCount; i++) {
				var player = players.GetChild (i);
				var diceCup = player.GetComponent<DiceCup> ();
				Debug.Log ("RpcLookUpAllDices");
				//diceCup.EvaluateDices ();
				//if (diceCup.isLocalPlayer) {
				Debug.Log ("isLocalPlayer = " + diceCup.isLocalPlayer);
				diceCup.EvaluateDices ();
				//}

				// send to client to look up the dices
				//if (EventLookUpAllDices != null) {
				//Debug.Log ("Send LookUpAllDices");
				//EventLookUpAllDices ();
				//}
			}
		}

		[ClientRpc]
		public void RpcHideAllDices () {
			for (int i = 0; i < players.childCount; i++) {
				var player = players.GetChild (i);
				var diceCup = player.GetComponent<DiceCup> ();

				if (diceCup.isLocalPlayer) {
					diceCup.HideDices ();
				}
			}
		}

		[Server]
		public void DecreaseDiceFromOtherPlayers () {
			for (int i = 0; i < players.childCount; i++) {
				if (playerIndex != i) {
					Transform player = players.GetChild (i);
					player.GetComponent<DiceCup> ().CmdDecreaseDiceFromPlayer ();
				}
			}
		}





		// ----- Helper -------------------------------------------------------------------------------------------------------------------------

		[Server]
		bool MayAct (ActionMessage msg) {
			GamePlayer gamePlayer = CheckRequestSource (msg);

			if (gamePlayer == null) {
				return false;
			}

			if (gameState is Bidding) {
				if (gamePlayer.isMyTurn) {
					return true;
				}
			}

			return false;
		}

		[Server]
		GamePlayer CheckRequestSource (ActionMessage msg) {
			Transform currentPlayer = GetCurrentPlayer ();
			GamePlayer gamePlayer = currentPlayer.GetComponent<GamePlayer> ();
			int connectionId = gamePlayer.connectionToClient.connectionId;

			if (connectionId == msg.connectionId) {
				return gamePlayer;
			}

			return null;
		}








		// ----- Camera -------------------------------------------------------------------------------------------------------------------------

		public void EnablePlayerCams () {
			if (isServer) {
				RpcEnablePlayerCams ();
			}
		}

		[ClientRpc]
		void RpcEnablePlayerCams () {
			//Transform players = players;
			enabledPlayerCams = new GamePlayer[players.childCount];

			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);
				GamePlayer gamePlayer = player.GetComponent<GamePlayer> ();

				int playerIndexDividedByTwo = Mathf.Abs (i / 2); // i[0,1]=0 i[2,3]=1
				int lastTwoPlayersAreOne = Mathf.Clamp (Mathf.Abs (i + 2 / players.childCount), 0, 1); // (2/1)=1; (2/2)=1,(3/2)=1; (2/3)=0,(3/3)=1,(4/3)=1, (2/4)=0,(3/4)=0,(4/4)=1,(5/4)=1
				int remainder = i % 2; // i[0,2]=0 i[1,3]=1
				float threePlayers = (players.childCount == 3 && i == 1) ? 0.5f : 0f;

				//int playersRemainder = players.childCount % 2f; // [1,3]=1 [2,4]=0
				//int s = Mathf.Abs (players.childCount / 3f); // [1,2]=0, [3,4]=1
				//int t = Mathf.Abs (players.childCount / 2f); // [1]=0, [2,3]=1, [4]=2

				/*
			new Rect (0f, 0f, 1f, 1f); // alles
				
			new Rect (0f, 0f, 0.5f, 1f); // links
			new Rect (0.5f, 0f, 0.5f, 1f); // rechts

			new Rect (0f, 0.5f, 0.5f, 0.5f); // oben links
			new Rect (0.5f, 0f, 0.5f, 1f); // rechts
			new Rect (0f, 0f, 0.5f, 0.5f); // unten links

			new Rect (0f, 0.5f, 0.5f, 0.5f); // oben links
			new Rect (0.5f, 0.5f, 0.5f, 0.5f); // oben rechts
			new Rect (0f, 0f, 0.5f, 0.5f); // unten links
			new Rect (0.5f, 0f, 0.5f, 0.5f); // unten rechts
			*/

				float x = remainder * 0.44f;
				float y = 0.445f - (lastTwoPlayersAreOne * 0.445f);
				float width = 0.44f * (3 - Mathf.Clamp (players.childCount, 1, 2));
				float height = (0.89f - (playerIndexDividedByTwo * 0.445f)) + threePlayers;
				Rect rect = new Rect (x, y, width, height);
				Debug.LogWarning ("Platziere Kamera: " + rect);

				gamePlayer.evaluationCam.rect = rect;
				gamePlayer.evaluationCam.gameObject.SetActive (true);
				//gamePlayer.cam.gameObject.SetActive (false);
				enabledPlayerCams [i] = gamePlayer;
			}
		}

		public void DisablePlayerCams () {
			if (isServer) {
				RpcDisablePlayerCams ();
			}
		}

		[ClientRpc]
		void RpcDisablePlayerCams () {
			//Transform players = players;

			for (int i = 0; i < enabledPlayerCams.Length; i++) {
				GamePlayer gamePlayer = enabledPlayerCams [i];

				gamePlayer.evaluationCam.gameObject.SetActive (false);

			}
		}

	}

}