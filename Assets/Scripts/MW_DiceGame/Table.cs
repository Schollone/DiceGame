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

		[SyncEvent]
		public event ControlbarButtonsDelegate EventUnlockControls;
		[SyncEvent]
		public event ControlbarButtonsDelegate EventLockControls;
		[SyncEvent]
		public event BidButtonsDelegate EventOnBidChanged;


		[SyncVar (hook = "OnGameStateChange")]
		public GameState theGameState;

		//[SyncVar (hook = "OnBidChange")]
		[SyncVar]
		public Bid currentBid;

		public static Table singleton;

		public Transform players;
		IState gameState;
		int playerIndex = 0;
		public IDictionary<DieFaces, int> dieFaceMap;
		int clientsReadyCount = 0;





		// ----- Callbacks --------------------------------------------------------------------------------------------------------------------------

		void Awake () {
			if (singleton == null) {
				singleton = this;
			} else if (singleton != this) {
				Destroy (gameObject);
			}

			dieFaceMap = new Dictionary<DieFaces, int> (Bid.maxBidDieFaceValue);
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
					gameState.SetActionStrategy (new CallOutBluff ());
					EnterEvaluationPhase ();
				}
			}
		}

		[Server]
		void OnDeclareBidSpotOn (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server erhält Anfrage für ein Verdacht auf ein präzises Gebot.");

			if (MayAct (msg)) {
				if (currentBid.Exists ()) {
					gameState.SetActionStrategy (new DeclareBidSpotOn ());
					EnterEvaluationPhase ();
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
			SetGameState (new Preparation (this));
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
			return players.GetChild (playerIndex);
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

		[Server]
		public void LookUpAllDices () {
			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);

				DiceCup diceCup = player.GetComponent<DiceCup> ();
				diceCup.EvaluateDices ();
				diceCup.RpcLookUpDices ();
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




	}

}