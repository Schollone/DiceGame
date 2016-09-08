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

		public delegate void ActionButtonsDelegate (bool bidAlreadyExists);

		public static event ControlbarButtonsDelegate UnlockControlsEvent;
		public static event ControlbarButtonsDelegate LockControlsEvent;
		public static event ActionButtonsDelegate OnBidChangedEvent;


		[SyncVar (hook = "OnGameStateChange")]
		public GameState theGameState;

		[SyncVar (hook = "OnBidChange")]
		public Bid currentBid;

		public static Table singleton;

		public Transform players;
		IState gameState;
		int playerIndex = 0;
		public IDictionary<DieFaces, int> dieFaceMap;
		int clientsReadyCount = 1;


		// ----- Callbacks ----- //

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

			//InitPlayers ();

		}

		public override void OnStartClient () {
			base.OnStartClient ();
			Debug.Log ("OnStartClient");

			/*if (isServer) {
				return;
			}*/

			InitPlayers ();
			this.currentBid = currentBid;
			if (OnBidChangedEvent != null) {
				OnBidChangedEvent (BidAlreadyExists ());
			}
		}

		void Start () {
			Debug.Log ("Start");
			SetGameState (new Preparation (this));
		}


		// ----- Hooks ----- //

		void OnGameStateChange (GameState state) {
			Debug.Log ("Setze GameState auf " + state);
			this.theGameState = state;

			if (state.Equals (GameState.Preparation) || state.Equals (GameState.GameOver)) {
				return;
			}

			DiceCup diceCup = null;

			for (int i = 0; i < players.childCount; i++) {
				var playerDiceCup = players.GetChild (i).GetComponent<DiceCup> ();
				if (playerDiceCup.isLocalPlayer) {
					diceCup = playerDiceCup;
					break;
				}
			}

			if (state.Equals (GameState.Bidding)) {
				diceCup.HideDices ();
				if (UnlockControlsEvent != null) {
					Debug.Log ("Aktiviere Look und Hide Buttons");
					UnlockControlsEvent ();
				}

			} else {
				diceCup.HideDices ();
				SendLockControlsEvent ();
			}
		}

		[Client]
		void OnBidChange (Bid bid) {
			//Debug.Log ("Table - OnBidChange: " + bid);
			if (isServer) {
				//return;
			}
				
			ChangeBidOnClients (bid);
		}

		[Client]
		void ChangeBidOnClients (Bid bid) {
			this.currentBid = bid;
			Debug.LogWarningFormat ("OnBidChange {0} - SendBidDoesNotExistEvent - isMyTurn: {1}", bid, true);

			//if (!BidAlreadyExists () && BidDoesNotExistEvent != null) {
			//var isMyTurn = this.GetCurrentPlayer ().GetComponent<GamePlayer> ().isMyTurn;
			if (OnBidChangedEvent != null) {
				OnBidChangedEvent (BidAlreadyExists ());
			}
		}

		public void SendLockControlsEvent () {
			Debug.LogWarning ("Deaktiviere alle Buttons");
			if (LockControlsEvent != null) {
				LockControlsEvent ();
			}
		}

		/*public void SendOnBidChangedEvent (bool isMyTurn) {
			Debug.LogWarning ("Es gibt kein Gebot. Sende Event! - isMyTurn: " + isMyTurn);
			if (OnBidChangedEvent != null) {
				OnBidChangedEvent ();
			}
		}*/





		// ----- GameStates ----- //

		[Server]
		public IState GetGameState () {
			return this.gameState;
		}

		[Server]
		public void SetGameState (IState gameState) {
			Debug.Log ("Setze GameState auf " + gameState);
			this.gameState = gameState;
			string stateName = gameState.GetType ().ToString ();
			//Debug.Log ("StateName = " + stateName);
			this.theGameState = (GameState)Enum.Parse (typeof(GameState), stateName);

			this.gameState.Execute ();
		}

		void InitPlayers () {
			//Debug.Log ("InitPlayers");
			players = GameObject.Find ("Player").transform;
		}

		[Server]
		public void StartGame () {
			Debug.LogWarning ("StartGame");
			this.gameState.StartGame ();
		}

		[Server]
		public void NextPlayer () {
			Debug.LogWarning ("NextPlayer");
			this.gameState.NextPlayer ();

			Transform player = GetCurrentPlayer ();
			player.GetComponent<GamePlayer> ().CmdItIsYourTurn (false);

			if (playerIndex + 1 < players.childCount) {
				playerIndex++;
			} else {
				playerIndex = 0;
			}

			player = GetCurrentPlayer ();
			player.GetComponent<GamePlayer> ().CmdItIsYourTurn (true);
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

		[Server]
		public void ThrowDices () {
			Debug.Log ("ThrowDices " + players.childCount);
			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);
				player.GetComponent<DiceCup> ().CmdFillDiceCupWithDices ();
			}
		}

		[Server]
		public void CountDicesOnTable () {
			dieFaceMap.Clear ();

			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);
				SpawnManager spawnManager = player.GetComponent<SpawnManager> ();
				DieFaces[] dieFaces = spawnManager.GetDieFacesFromPlayer ();
				for (int j = 0; j < dieFaces.Length; j++) {

					DieFaces dieFace = dieFaces [j];
					int value = 1;

					if (this.dieFaceMap.ContainsKey (dieFace)) {
						value = dieFaceMap [dieFace];
						value++;
						dieFaceMap [dieFace] = value;
						//dieFaceMap.Add (dieFace, value + 1);
					} else {
						dieFaceMap.Add (dieFace, value);
					}

				}

			}
		}

		// ----- Action Handler on Server ----- //

		[Server]
		void OnClientReady (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			//Debug.Log ("Server Table - OnClientReady " + msg);

			for (int i = 0; i < players.childCount; i++) {
				var player = players.GetChild (i);
				GamePlayer gamePlayer = player.GetComponent<GamePlayer> ();
				int connectionId = gamePlayer.connectionToClient.connectionId;

				if (connectionId == msg.connectionId) {
					this.clientsReadyCount++;
					break;
				}
			}

			//Debug.Log ("clientsReady=" + clientsReadyCount + " von insgesamt " + players.childCount + " Clients.");
			if (this.clientsReadyCount == players.childCount) {
				//Debug.Log ("Alle Clients are ready to fill up dice cups");
				//Invoke ("StartGame", 3f);// StartGame ();
				StartGame ();
			}

		}

		[Server]
		void OnEnterBid (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server Table - OnEnterBid " + msg);

			if (MayAct (msg)) {
				gameState.SetActionStrategy (new EnterBid (msg.bid));
				gameState.GetActionStrategy ().ExecuteAction (this);
				NextPlayer ();
			}
		}

		[Server]
		void OnCallOutBluff (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server Table - OnCallOutBluff " + msg);

			if (MayAct (msg)) {
				if (BidAlreadyExists ()) {
					gameState.SetActionStrategy (new CallOutBluff ());
					EnterEvaluationPhase ();
				}
			}
		}

		[Server]
		void OnDeclareBidSpotOn (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server Table - OnDeclareBidSpotOn " + msg);

			if (MayAct (msg)) {
				if (BidAlreadyExists ()) {
					gameState.SetActionStrategy (new DeclareBidSpotOn ());
					EnterEvaluationPhase ();
				}
			}
		}

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

		public bool BidAlreadyExists () {
			//Debug.Log ("currentBid.dieFace " + currentBid.dieFace);
			if (currentBid.dieFace != DieFaces.Null) {
				return true;
			}

			return false;
		}

		[Server]
		public void ResetBid () {
			this.currentBid = new Bid (DieFaces.Null, 0);
		}

		[Server]
		public void LookUpAllDices () {
			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);

				DiceCup diceCup = player.GetComponent<DiceCup> ();
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

	}

}