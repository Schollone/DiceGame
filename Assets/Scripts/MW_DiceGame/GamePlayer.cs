using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace MW_DiceGame {

	public class GamePlayer : NetworkBehaviour {

		[SyncVar]
		public string playerName;
		[SyncVar]
		public Colors color;
		[SyncVar]
		public Slots slotId;
		[SyncVar (hook = "OnIsMyTurn")]
		public bool isMyTurn;

		public Camera cam;

		public delegate void PlayerNameDelegate (Slots targetSlot, string newPlayerName);

		public delegate void ColorDelegate (Slots targetSlot, Colors newColor);

		//public delegate void ButtonControlsDelegate (bool isMyTurn);

		public static event PlayerNameDelegate PlayerNameChangedEvent;
		public static event ColorDelegate ColorChangedEvent;

		//public static event ButtonControlsDelegate ShowControlsEvent;
		//public static event ButtonControlsDelegate HideControlsEvent;
		//public static event ButtonControlsDelegate ActiveControlsEvent;
		//public static event ButtonControlsDelegate PassiveControlsEvent;


		public delegate void NextPlayerDelegate (bool isMyTurn);

		public static event NextPlayerDelegate ItIsMyTurnEvent;

		//IAction actionStrategy;



		public override void OnStartClient () {
			base.OnStartClient ();
			Debug.Log ("playerName=" + playerName);
			PutInContainer ("Player", gameObject);
			gameObject.name = "Player" + playerName;

			if (PlayerNameChangedEvent != null) {
				PlayerNameChangedEvent (slotId, playerName);
			}
			if (ColorChangedEvent != null) {
				ColorChangedEvent (slotId, color);
			}

			//OnIsMyTurn (isMyTurn);
		}

		public override void OnStartLocalPlayer () {
			base.OnStartLocalPlayer ();

			//BidController.OnEnterBid += OnEnterBid;
			//EventManager.OnCallOutBluff += OnCallOutBluff;
			//EventManager.OnDeclareSpotOn += OnDeclareBidSpotOn;

			//actionStrategy = null;

			cam.gameObject.SetActive (true);
		}

		public void OnIsMyTurn (bool isMyTurn) {
			this.isMyTurn = isMyTurn;

			if (!isLocalPlayer) {
				return;
			}

			if (Table.singleton.theGameState.Equals (Table.GameState.Bidding)) {
				
				if (!Table.singleton.BidAlreadyExists ()) {
					Table.singleton.SendBidDoesNotExistEvent (isMyTurn);
				} else {
					if (ItIsMyTurnEvent != null) {
						//Debug.Log (playerName + " --> ItIsMyTurnEvent: " + isMyTurn);
						ItIsMyTurnEvent (isMyTurn);
					}
				}
			}
			/* else {
				if (ShowControlsEvent != null) {
					Debug.Log ("ShowControlsEvent: " + isMyTurn);
					Debug.Log ("HideDices");
					GetComponent<DiceCup> ().HideDices ();
					ShowControlsEvent (isMyTurn);
				}
			}*/

		}

		void PutInContainer (string containerName, GameObject child) {
			var container = GameObject.Find (containerName);
			if (container == null) {
				container = new GameObject (containerName);
			}
			child.transform.SetParent (container.transform);
		}

		/*void SetActionStrategy (IAction strategy) {
			actionStrategy = strategy;
		}

		void OnEnterBid (Bid bidData) {
			Bid bid = new Bid (bidData.dieFace, bidData.quantity, this.netId);
			Debug.LogFormat ("Enter Bid {0}", bid);

			SetActionStrategy (new EnterBid (bid));
			HandleAction ();
		}

		void OnCallOutBluff () {
			Debug.LogFormat ("Call Out Bluff");

			SetActionStrategy (new CallOutBluff ());
			HandleAction ();
		}

		void OnDeclareBidSpotOn () {
			Debug.LogFormat ("Declare Bid Spot On");

			SetActionStrategy (new DeclareBidSpotOn ());
			HandleAction ();
		}

		void HandleAction () {
			Debug.LogFormat ("Handle Action {0}", actionStrategy.GetType ());

			// if status "Bidding"

			if (isMyTurn) {
				actionStrategy.ExecuteAction (Table.singleton);
			}

		}*/

		public override string ToString () {
			return string.Format ("[GamePlayer: playerName={0}, color={1}, slot={2}, isMyTurn={3}]", playerName, color, slotId, isMyTurn);
		}


		[Command]
		public void CmdItIsYourTurn (bool isMyTurn) {
			Debug.Log ("It is your turn: " + isMyTurn);
			this.isMyTurn = isMyTurn;
		}
	}

}