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
		public Camera evaluationCam;

		public SkinnedMeshRenderer cowboyRenderer;

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
			PutInContainer ("Player", gameObject);
			gameObject.name = "Player" + playerName;

			if (PlayerNameChangedEvent != null) {
				PlayerNameChangedEvent (slotId, playerName);
			}
			if (ColorChangedEvent != null) {
				ColorChangedEvent (slotId, color);
			}

			string path = "Player/Cowboy_" + color.ToString ();
			Material mat = Resources.Load<Material> (path);
			cowboyRenderer.material = mat;
		}

		public override void OnStartLocalPlayer () {
			Debug.Log ("OnStartLocalPlayer GamePlayer");
			base.OnStartLocalPlayer ();

			cam.gameObject.SetActive (true);


		}

		void Start () {
			Debug.Log ("Start GamePlayer");
			//Table.singleton.EventUnlockControls += OnUnlockControls;
			//Table.singleton.EventLockControls += OnLockControls;
			//Table.singleton.EventOnBidChanged += OnBidChanged;
		}

		void PutInContainer (string containerName, GameObject child) {
			var container = GameObject.Find (containerName);
			if (container == null) {
				container = new GameObject (containerName);
			}
			child.transform.SetParent (container.transform);
		}

		[ClientRpc]
		public void RpcEliminatePlayer (NetworkInstanceId id) {
			if (netId.Equals (id)) {
				PutInContainer ("Eliminated", gameObject);
				if (isLocalPlayer) {
					GetComponent<DiceCup> ().Eliminated ();
				}
			}
		}

		public override string ToString () {
			return string.Format ("[GamePlayer: playerName={0}, color={1}, slot={2}, isMyTurn={3}]", playerName, color, slotId, isMyTurn);
		}

		[Command]
		public void CmdItIsYourTurn (bool isMyTurn) {
			if (isMyTurn) {
				Debug.Log (playerName + " ist an der Reihe!");
			} else {
				Debug.Log (playerName + " ist nicht mehr an der Reihe!");
			}

			this.isMyTurn = isMyTurn;
		}







		[ClientRpc]
		public void RpcUnlockControls () {
			if (isLocalPlayer) {
				OnUnlockControls ();
			}
		}

		void OnUnlockControls () {
			if (EventUnlockControls != null) {
				Debug.Log ("GamePlayer - Schicke Event zu BidController");
				EventUnlockControls (isMyTurn, Table.singleton.currentBid.Exists ());
			}
		}

		[ClientRpc]
		public void RpcLockControls () {
			if (isLocalPlayer) {
				OnLockControls ();
			}
		}

		void OnLockControls () {
			if (EventLockControls != null) {
				EventLockControls (isMyTurn, Table.singleton.currentBid.Exists ());
			}
		}

		[ClientRpc]
		public void RpcOnBidChanged (Bid bid) {
			OnBidChanged (bid);
		}

		void OnBidChanged (Bid newBid) {
			Table.singleton.currentBid = newBid;

			if (EventOnBidChanged != null) {
				EventOnBidChanged (isMyTurn, Table.singleton.currentBid.Exists ());
			}
		}

		void OnIsMyTurn (bool isMyTurn) {
			this.isMyTurn = isMyTurn;

			if (isLocalPlayer) {
				if (ItIsMyTurnEvent != null) {
					ItIsMyTurnEvent (isMyTurn, Table.singleton.currentBid.Exists ());
				}
			}
		}

		void OnDestroy () {
			//Table.singleton.EventUnlockControls -= OnUnlockControls;
			//Table.singleton.EventLockControls -= OnLockControls;
			//Table.singleton.EventOnBidChanged -= OnBidChanged;
		}
	}

}