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

		public delegate void EventDisplayDelegate (string playerName, Colors color, string actionDescription);

		public static event EventDisplayDelegate UpdateEventDisplayEvent;

		public delegate void EventDisplayVisibilityDelegate (bool visibility);

		public static event EventDisplayVisibilityDelegate EventDisplayVisibilityEvent;

		/*[SyncEvent]
		public event ControlbarButtonsDelegate EventUnlockControls;
		[SyncEvent]
		public event ControlbarButtonsDelegate EventLockControls;
		[SyncEvent]
		public event BidButtonsDelegate EventOnBidChanged;*/

		[SyncVar]
		public Bid currentBid;

		public static Table singleton;

		[HideInInspector] public Transform players;
		IState gameState;
		int playerIndex = 0;
		[HideInInspector] public IDictionary<DieFaces, int> dieFaceMap;

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

			NetworkServer.RegisterHandler (ActionMsg.ClientReady, OnClientReady);
			NetworkServer.RegisterHandler (ActionMsg.EnterBid, OnEnterBid);
			NetworkServer.RegisterHandler (ActionMsg.CallOutBluff, OnCallOutBluff);
			NetworkServer.RegisterHandler (ActionMsg.DeclareBidSpotOn, OnDeclareBidSpotOn);
		}

		public override void OnStartClient () {
			base.OnStartClient ();
			InitPlayers ();
		}

		void Start () {
			if (isServer) {
				Invoke ("StartGame", 3f);
			}
		}





		// ----- Hooks --------------------------------------------------------------------------------------------------------------------------

		[Server]
		public void SendUnlockControlsEvent () {
			//Debug.Log ("SendUnlockControlsEvent");
			for (int i = 0; i < players.childCount; i++) {
				players.GetChild (i).GetComponent<GamePlayer> ().RpcUnlockControls ();
			}

			/*if (EventUnlockControls != null) {
			Debug.Log ("Send Unlock Controls from Server");
			EventUnlockControls ();
			}*/
		}

		[Server]
		public void SendLockControlsEvent () {
			for (int i = 0; i < players.childCount; i++) {
				players.GetChild (i).GetComponent<GamePlayer> ().RpcLockControls ();
			}

			/*if (EventLockControls != null) {
				Debug.Log ("Send Lock Controls from Server");
				EventLockControls ();
			}*/
		}

		[Server]
		public void ChangeBidOnClients (Bid bid) {
			for (int i = 0; i < players.childCount; i++) {
				players.GetChild (i).GetComponent<GamePlayer> ().RpcOnBidChanged (bid);
			}

			/*if (EventOnBidChanged != null) {
				EventOnBidChanged (bid);
			}*/
		}





		// ----- Network Handler ----------------------------------------------------------------------------------------------------------------

		[Server]
		public void OnClientReady (NetworkMessage netMsg) {
		}

		[Server]
		void OnEnterBid (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server erhält Anfrage für ein neues Gebot: " + msg.bid);

			if (MayAct (msg)) {
				gameState.SetActionStrategy (new EnterBid (msg.bid));
				Execute ();
				var gp = GetCurrentPlayer ().GetComponent<GamePlayer> ();
				RpcTellClientsCurrentAction (gp.playerName, gp.color, "entered a bid.");
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
					var gp = GetCurrentPlayer ().GetComponent<GamePlayer> ();
					RpcTellClientsCurrentAction (gp.playerName, gp.color, "called out bluff.");
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
					var gp = GetCurrentPlayer ().GetComponent<GamePlayer> ();
					RpcTellClientsCurrentAction (gp.playerName, gp.color, "declared bid spot on.");
				}
			}
		}

		[ClientRpc]
		public void RpcTellClientsCurrentAction (string playerName, Colors color, string actionDecription) {
			//if (gamePlayer != null) {
			UpdateEventDisplayEvent (playerName, color, actionDecription);
			//}
		}

		[ClientRpc]
		public void RpcTellClientsToDisableActionText () {
			EventDisplayVisibilityEvent (false);
		}

		[ClientRpc]
		public void RpcTellClientsToEnableActionText () {
			EventDisplayVisibilityEvent (true);
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
			}
		}





		// ----- Player -------------------------------------------------------------------------------------------------------------------------

		void InitPlayers () {
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
		public Transform GetNextPlayer () {
			int nextPlayerIndex = playerIndex + 1;
			if (nextPlayerIndex >= players.childCount) {
				nextPlayerIndex = 0;
			}
			return players.GetChild (nextPlayerIndex);
		}

		[Server]
		public Transform GetPrevPlayer () {
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
			gamePlayer.CmdItIsYourTurn (false);
		}

		[Server]
		void UpdatePlayerIndex () {
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

				if (diceCup.isLocalPlayer) {
					diceCup.EvaluateDices ();
				}
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

			Debug.Log (connectionId + " == " + msg.connectionId);
			return gamePlayer;
		}








		// ----- Camera -------------------------------------------------------------------------------------------------------------------------

		public void EnablePlayerCams () {
			if (isServer) {
				RpcEnablePlayerCams ();
			}
		}

		[ClientRpc]
		void RpcEnablePlayerCams () {
			enabledPlayerCams = new GamePlayer[players.childCount];

			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);
				GamePlayer gamePlayer = player.GetComponent<GamePlayer> ();

				int playerIndexDividedByTwo = Mathf.Abs (i / 2); // i[0,1]=0 i[2,3]=1
				int lastTwoPlayersAreOne = Mathf.Clamp (Mathf.Abs (i + 2 / players.childCount), 0, 1); // (2/1)=1; (2/2)=1,(3/2)=1; (2/3)=0,(3/3)=1,(4/3)=1, (2/4)=0,(3/4)=0,(4/4)=1,(5/4)=1
				int remainder = i % 2; // i[0,2]=0 i[1,3]=1
				float threePlayers = (players.childCount == 3 && i == 1) ? 0.5f : 0f;

				float x = remainder * 0.44f;
				float y = 0.445f - (lastTwoPlayersAreOne * 0.445f);
				float width = 0.44f * (3 - Mathf.Clamp (players.childCount, 1, 2));
				float height = (0.89f - (playerIndexDividedByTwo * 0.445f)) + threePlayers;
				Rect rect = new Rect (x, y, width, height);

				gamePlayer.evaluationCam.rect = rect;
				gamePlayer.evaluationCam.gameObject.SetActive (true);
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

			for (int i = 0; i < enabledPlayerCams.Length; i++) {
				GamePlayer gamePlayer = enabledPlayerCams [i];
				gamePlayer.evaluationCam.gameObject.SetActive (false);
			}
		}

	}

}