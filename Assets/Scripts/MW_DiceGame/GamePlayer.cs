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

		public static event PlayerNameDelegate PlayerNameChangedEvent;
		public static event ColorDelegate ColorChangedEvent;



		public delegate void ControlbarButtonsDelegate (bool isMyTurn, bool bidAlreadyExists);

		public static event ControlbarButtonsDelegate EventUnlockControls;
		public static event ControlbarButtonsDelegate EventLockControls;
		public static event ControlbarButtonsDelegate EventOnBidChanged;
		public static event ControlbarButtonsDelegate ItIsMyTurnEvent;


		public override void OnStartClient () {
			base.OnStartClient ();
			//Debug.Log ("playerName=" + playerName);
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

			cam.gameObject.SetActive (true);
		}

		void Start () {
			Debug.Log ("Start GamePlayer - Client active: " + NetworkClient.active + " Server active: " + NetworkServer.active);

			Table.singleton.EventUnlockControls += OnUnlockControls;
			Table.singleton.EventLockControls += OnLockControls;
			Table.singleton.EventOnBidChanged += OnBidChanged;
		}

		void PutInContainer (string containerName, GameObject child) {
			var container = GameObject.Find (containerName);
			if (container == null) {
				container = new GameObject (containerName);
			}
			child.transform.SetParent (container.transform);
		}

		public override string ToString () {
			return string.Format ("[GamePlayer: playerName={0}, color={1}, slot={2}, isMyTurn={3}]", playerName, color, slotId, isMyTurn);
		}


		[Command]
		public void CmdItIsYourTurn (bool isMyTurn) {
			//Debug.Log ("CmdItIsYourTurn: " + isMyTurn);

			if (isMyTurn) {
				Debug.Log (playerName + " ist an der Reihe!");
			} else {
				Debug.Log (playerName + " ist nicht mehr an der Reihe!");
			}

			this.isMyTurn = isMyTurn;
		}









		void OnUnlockControls () {
			//if (isLocalPlayer) {
			Debug.Log ("Aktiviere Look und Hide Buttons");
			if (EventUnlockControls != null) {
				EventUnlockControls (isMyTurn, Table.singleton.currentBid.Exists ());
			}
			//}
		}

		void OnLockControls () {
			//if (isLocalPlayer) {
			Debug.Log ("Deaktiviere alle Buttons");
			if (EventLockControls != null) {
				EventLockControls (isMyTurn, Table.singleton.currentBid.Exists ());
			}
			//}
		}

		void OnBidChanged (Bid newBid) {
			//if (isLocalPlayer) {
			Debug.Log ("Neues Gebot vom Server vekommen: " + newBid);
			Table.singleton.currentBid = newBid;

			if (EventOnBidChanged != null) {
				EventOnBidChanged (isMyTurn, Table.singleton.currentBid.Exists ());
			}
			//}
		}

		public void OnIsMyTurn (bool isMyTurn) {
			this.isMyTurn = isMyTurn;

			if (isLocalPlayer) {
				Debug.Log ("OnIsMyTurn - ItIsMyTurn: " + isMyTurn);
				if (ItIsMyTurnEvent != null) {
					ItIsMyTurnEvent (isMyTurn, Table.singleton.currentBid.Exists ());
				}
			}

		}

		void OnDestroy () {
			Table.singleton.EventUnlockControls -= OnUnlockControls;
			Table.singleton.EventLockControls -= OnLockControls;
			Table.singleton.EventOnBidChanged -= OnBidChanged;
		}
	}

}