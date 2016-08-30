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



		[SyncVar]
		public GameState theGameState;

		[SyncVar (hook = "OnBidChange")]
		public Bid currentBid;

		public static Table singleton;

		Transform players;
		IState gameState;
		int playerIndex = 0;
		IDictionary<DieFaces, int> dieFaceMap;

		void OnBidChange (Bid bid) {
			Debug.Log ("Table - OnBidChange: " + bid);
			this.currentBid = bid;
		}





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

			NetworkServer.RegisterHandler (ActionMsg.EnterBid, OnEnterBid);
			NetworkServer.RegisterHandler (ActionMsg.CallOutBluff, OnCallOutBluff);
			NetworkServer.RegisterHandler (ActionMsg.DeclareBidSpotOn, OnDeclareBidSpotOn);

			SetGameState (new Preparation (this));
			StartGame ();
		}

		public override void OnStartClient () {
			base.OnStartClient ();
			OnBidChange (currentBid);
		}

		[Server]
		public IState GetGameState () {
			return this.gameState;
		}

		[Server]
		public void SetGameState (IState gameState) {
			this.gameState = gameState;
			string stateName = gameState.GetType ().ToString ();
			Debug.Log ("StateName = " + stateName);
			this.theGameState = (GameState)Enum.Parse (typeof(GameState), stateName);
		}

		[Server]
		public void InitPlayers () {
			Debug.Log ("InitPlayers");
			players = GameObject.Find ("Player").transform;
			Transform player = GetCurrentPlayer ();
			player.GetComponent<GamePlayer> ().CmdItIsYourTurn (true);
		}

		[Server]
		public void StartGame () {
			Debug.Log ("StartGame");
			this.gameState.StartGame ();
		}

		[Server]
		public void NextPlayer () {
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



		[Server]
		void OnEnterBid (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server Table - OnEnterBid " + msg);

			if (MayAct (msg)) {
				if (BidAlreadyExists ()) {
					if (this.currentBid.CanBeReplacedWith (msg.bid)) {
						this.currentBid = msg.bid;
					}
				} else {
					this.currentBid = msg.bid;
				}

				NextPlayer ();
			}
		}

		[Server]
		void OnCallOutBluff (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server Table - OnCallOutBluff " + msg);

			if (MayAct (msg)) {

				if (BidAlreadyExists ()) {
					EnterEvaluationPhase ();
					
					CountDicesOnTable ();
					LookUpAllDices ();

					int value = 0;
					this.dieFaceMap.TryGetValue (currentBid.dieFace, out value);
					Bid realBidOnTable = new Bid (currentBid.dieFace, value);

					Transform player;

					if (currentBid.IsBluff (realBidOnTable)) {
						Debug.Log ("Table - Bluff: last player loses a dice.");
						player = GetLastPlayer ();
					} else {
						Debug.Log ("Table - No Bluff: current player " + GetCurrentPlayer ().GetComponent<GamePlayer> ().playerName + " loses a dice.");
						player = GetCurrentPlayer ();
					}

					DiceCup diceCup = player.GetComponent<DiceCup> ();
					diceCup.CmdDecreaseDiceFromPlayer ();

					EnterBidding ();
					NextPlayer ();
				}
			}
		}

		[Server]
		void OnDeclareBidSpotOn (NetworkMessage netMsg) {
			var msg = netMsg.ReadMessage<ActionMessage> ();
			Debug.Log ("Server Table - OnDeclareBidSpotOn " + msg);

			if (MayAct (msg)) {
				
				if (BidAlreadyExists ()) {
					EnterEvaluationPhase ();

					CountDicesOnTable ();
					LookUpAllDices ();

					int value = 0;
					this.dieFaceMap.TryGetValue (currentBid.dieFace, out value);
					Bid realBidOnTable = new Bid (currentBid.dieFace, value);

					if (this.currentBid.IsSpotOn (realBidOnTable)) {
						Debug.Log ("Table - IsSpotOn: Everybody loses a dice!");
						DecreaseDiceFromOtherPlayers ();
					} else {
						Debug.Log ("Table - Not Spot On: " + GetCurrentPlayer ().GetComponent<GamePlayer> ().playerName + " loses a dice");
						Transform player = GetCurrentPlayer ();
						DiceCup diceCup = player.GetComponent<DiceCup> ();
						diceCup.CmdDecreaseDiceFromPlayer ();
					}

					EnterBidding ();
					NextPlayer ();
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
		Transform GetCurrentPlayer () {
			return players.GetChild (playerIndex);
		}

		[Server]
		Transform GetLastPlayer () {
			int lastPlayerIndex = playerIndex - 1;
			if (lastPlayerIndex < 0) {
				lastPlayerIndex = players.childCount - 1;
			}
			return players.GetChild (lastPlayerIndex);
		}

		[Server]
		bool BidAlreadyExists () {
			if (currentBid.dieFace != DieFaces.Null) {
				return true;
			}

			return false;
		}

		[Server]
		void LookUpAllDices () {
			for (int i = 0; i < players.childCount; i++) {
				Transform player = players.GetChild (i);

				DiceCup diceCup = player.GetComponent<DiceCup> ();
				diceCup.LookUpDices ();
			}
		}

		[Server]
		void DecreaseDiceFromOtherPlayers () {
			for (int i = 0; i < players.childCount; i++) {
				if (playerIndex != i) {
					Transform player = players.GetChild (i);
					player.GetComponent<DiceCup> ().CmdDecreaseDiceFromPlayer ();
				}
			}
		}

	}

}