using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

namespace MW_DiceGame {

	public class DiceCup : NetworkBehaviour {

		public static readonly int maxDices = 5;

		public delegate void AvailableDicesDelegate (Slots targetSlot, int dicesLeft);

		public static event AvailableDicesDelegate AvailableDicesChangedEvent;

		[SyncVar (hook = "OnAvailableDicesChanged")]
		public int availableDices;

		public Animator anim;
		public Transform diceSpawnPoint;
		public float speed = 60f;
		public string LookButtonName = "lookButtonName";
		public string HideButtonName = "hideButtonName";

		[SerializeField]
		Rect rect1;
		[SerializeField]
		Rect rect2;

		SpawnManager spawnManager;

		GameObject lookBtn;
		GameObject hideBtn;

		GamePlayer gamePlayer;

		int[] dices = new int[maxDices];



		void Awake () {
			spawnManager = GetComponent<SpawnManager> ();
		}

		public override void OnStartClient () {
			Debug.Log ("DiceCup - OnStartClient");
			base.OnStartClient ();

			//spawnManager = GetComponent<SpawnManager> ();
			gamePlayer = GetComponent<GamePlayer> ();

			rect1 = rect2 = new Rect ();

			OnAvailableDicesChanged (availableDices);
		}

		public override void OnStartLocalPlayer () {
			Debug.Log ("DiceCup - OnStartLocalPlayer");
			base.OnStartLocalPlayer ();

			lookBtn = GameObject.Find (LookButtonName);
			hideBtn = GameObject.Find (HideButtonName);

			if (hideBtn != null) {
				hideBtn.SetActive (false);
			}

			EventManager.OnLook += LookUpDices;
			EventManager.OnHide += HideDices;
		}

		void OnAvailableDicesChanged (int availableDices) {
			Debug.Log ("DiceCup - OnAvailableDicesChange " + availableDices);
			this.availableDices = availableDices;
			if (AvailableDicesChangedEvent != null) {
				AvailableDicesChangedEvent (GetComponent<GamePlayer> ().slotId, availableDices);
			}
		}



		[ClientCallback]
		void Update () {
			if (!isLocalPlayer) {
				return;
			}

			if (Input.GetKeyUp (KeyCode.L)) {
				CmdDecreaseAvailableDices ();
			}

			if (Input.GetKeyUp (KeyCode.F)) {
				CmdFillDiceCupWithDices ();
			}
		}




		[Command]
		public void CmdDecreaseAvailableDices () {
			if (availableDices > 0) {
				availableDices = availableDices - 1;

				GameObject go = spawnManager.diceContainer.transform.GetChild (0).gameObject;

				//GameObject go = spawnManager.dices.GetItem (0).dice;
				//GameObject go = spawnManager.pool [availableDices];

				spawnManager.UnSpawnHandler (go);
				NetworkServer.UnSpawn (go);

				//if (OnLostDice != null) {
				//OnLoseDice ();
				//}

			} else {
				// eliminate Player;
			}
		}

		[Command]
		public void CmdFillDiceCupWithDices () {
			spawnManager.UnspawnAllObjects ();

			for (int i = 0; i < availableDices; i++) {
				var diceGO = spawnManager.GetFromPool (diceSpawnPoint.position);

				Dice dice = diceGO.GetComponent<Dice> ();
				dice.diceCupId = netId;
				dice.color = gamePlayer.color;

				float angle = Random.Range (72 - 30, 72 + 30);
				diceSpawnPoint.Rotate (Vector3.up, angle, Space.World);

				diceGO.transform.rotation = Random.rotation;
				diceGO.GetComponent<Rigidbody> ().AddForce (diceSpawnPoint.up * speed * Time.deltaTime, ForceMode.Impulse);

				NetworkServer.Spawn (diceGO, spawnManager.assetId);
			}
		}





		public void LookUpDices () {
			Debug.LogFormat ("Look Up Dices");
			if (!isLocalPlayer) {
				return;
			}
			lookBtn.SetActive (false);
			hideBtn.SetActive (true);
			anim.SetBool ("looking", true);
		}

		public void HideDices () {
			Debug.LogFormat ("Hide Dices");
			if (!isLocalPlayer) {
				return;
			}
			lookBtn.SetActive (true);
			hideBtn.SetActive (false);
			anim.SetBool ("looking", false);
		}

		void OnDestroy () {
			EventManager.OnLook -= LookUpDices;
			EventManager.OnHide -= HideDices;
		}

		void OnGUI () {
			//Debug.Log (availableDices.ToString ());

			GUI.color = Color.white;
			GUI.Label (rect1, gamePlayer.ToString () + " -> Dices: " + availableDices.ToString ());

			if (dices.Length == 0) {
				return;
			}

			Rect tmpRect = rect2;

			GUI.Label (tmpRect, dices [0].ToString ());
			tmpRect.x += 20;
			GUI.Label (tmpRect, dices [1].ToString ());
			tmpRect.x += 20;
			GUI.Label (tmpRect, dices [2].ToString ());
			tmpRect.x += 20;
			GUI.Label (tmpRect, dices [3].ToString ());
			tmpRect.x += 20;
			GUI.Label (tmpRect, dices [4].ToString ());

		}

		public void UpdateDieFaceValueDisplay () {
			Debug.Log ("UpdateDieFaceValueDisplay()");

			rect1 = new Rect (150, gamePlayer.slotId.GetIndex () * 20 + 100, 320, 100);
			rect2 = new Rect (20, GetComponent<GamePlayer> ().slotId.GetIndex () * 20 + 100, 300, 100);
			//dices = spawnManager.GetDiceValues ();
		}

	}

}