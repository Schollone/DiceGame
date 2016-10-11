using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

namespace MW_DiceGame {

	public class DiceCup : NetworkBehaviour {

		public GamePlayer gamePlayer;
		public SpawnManager spawnManager;

		public static readonly int maxDices = 5;

		public delegate void AvailableDicesDelegate (Slots targetSlot, int dicesLeft);

		public static event AvailableDicesDelegate AvailableDicesChangedEvent;

		[SyncVar (hook = "OnAvailableDicesChanged")]
		public int availableDices;

		public Animator anim;
		public GameObject diceSpawnPoints;
		public float speed = 60f;
		public string LookButtonName = "lookButtonName";
		public string HideButtonName = "hideButtonName";

		[SerializeField]
		Rect rect1;
		[SerializeField]
		Rect rect2;

		GameObject lookBtn;
		GameObject hideBtn;

		int[] dices = new int[maxDices];
		Transform[] spawnPoints;


		public override void OnStartClient () {
			base.OnStartClient ();
			rect1 = rect2 = new Rect ();

			OnAvailableDicesChanged (availableDices);

			spawnPoints = new Transform[maxDices];
			for (int i = 0; i < spawnPoints.Length; i++) {
				spawnPoints [i] = diceSpawnPoints.transform.GetChild (i);
			}
		}

		public override void OnStartLocalPlayer () {
			base.OnStartLocalPlayer ();

			lookBtn = GameObject.Find (LookButtonName);
			hideBtn = GameObject.Find (HideButtonName);

			if (hideBtn != null) {
				hideBtn.SetActive (false);
			}

			ClickManager.OnLook += LookUpDices;
			ClickManager.OnHide += HideDices;
		}

		void Start () {
		}

		void OnAvailableDicesChanged (int availableDices) {
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
				CmdDecreaseDiceFromPlayer ();
			}

			if (Input.GetKeyUp (KeyCode.F)) {
				CmdFillDiceCupWithDices ();
			}

		}


		[Command]
		public void CmdDecreaseDiceFromPlayer () {
			if (availableDices > 0) {
				DecreaseDice ();
			}
		}

		void DecreaseDice () {
			availableDices = availableDices - 1;

			GameObject go = spawnManager.container.transform.GetChild (0).gameObject;

			RpcPlayDiceSmoke (go.transform.position);

			spawnManager.UnSpawnHandler (go);
			NetworkServer.UnSpawn (go);

			if (availableDices <= 0) {
				Debug.Log ("Eliminate Player");
				gamePlayer.RpcEliminatePlayer (netId);

				if (Table.singleton.players.childCount > 1) {
					var gp = ClientScene.FindLocalObject (netId).GetComponent<GamePlayer> ();
					Table.singleton.RpcTellClientsCurrentAction (gp.playerName, gp.color, "is eliminated.");
				} else {
					Debug.Log ("GameOver");
					var gp = Table.singleton.players.GetChild (0).GetComponent<GamePlayer> ();
					Table.singleton.RpcTellClientsCurrentAction (gp.playerName, gp.color, "has won.");
					Table.singleton.LeaveGame ();
				}
			}
		}

		[ClientRpc]
		void RpcPlayDiceSmoke (Vector3 startposition) {
			PlayDiceSmoke (startposition);
		}

		void PlayDiceSmoke (Vector3 startPosition) {
			GameObject diceSmoke = GameObject.Find ("DiceSmoke");
			if (diceSmoke == null) {
				diceSmoke = (GameObject)GameObject.Instantiate (spawnManager.unspawnParticleEffectPrefab, startPosition, Quaternion.Euler (new Vector3 (-90f, 0f, 0f)));
			}
			diceSmoke.transform.position = startPosition;
			diceSmoke.SetActive (true);
			var particleSystem = diceSmoke.GetComponent<ParticleSystem> ();
			particleSystem.startColor = gamePlayer.color.GetColor ();
			particleSystem.Play ();
		}

		[Command]
		public void CmdFillDiceCupWithDices () {
			GameObject diceContainer = spawnManager.container;

			for (int i = 0; i < availableDices; i++) {
				ThrowDice (diceContainer, i);
			}

		}

		void ThrowDice (GameObject diceContainer, int i) {
			Transform diceGO = diceContainer.transform.GetChild (i);
			Dice dice = diceGO.GetComponent<Dice> ();
			dice.ResetDice ();

			diceGO.position = spawnPoints [i].position;
			diceGO.GetComponent<Rigidbody> ().isKinematic = false;

			diceGO.transform.rotation = Random.rotation;
		}

		public void Eliminated () {
			if (!isLocalPlayer) {
				return;
			}

			lookBtn.SetActive (false);
			hideBtn.SetActive (false);

			CmdEliminated ();
		}

		[Command]
		void CmdEliminated () {
			RpcEliminated ();
		}

		[ClientRpc]
		void RpcEliminated () {
			anim.SetTrigger ("Eliminate");
		}

		public void EvaluateDices () {
			if (!isLocalPlayer) {
				return;
			}

			lookBtn.SetActive (false);
			hideBtn.SetActive (false);
			CmdStartAnimationEvaluation ();
		}

		[Command]
		void CmdStartAnimationEvaluation () {
			RpcStartAnimationEvaluation ();
		}

		[ClientRpc]
		void RpcStartAnimationEvaluation () {
			anim.SetTrigger ("Evaluate");
		}

		public void LookUpDices () {
			if (!isLocalPlayer) {
				return;
			}
				
			lookBtn.SetActive (false);
			hideBtn.SetActive (true);
			anim.SetBool ("looking", true);
		}

		[Command]
		void CmdStartAnimationLookUp () {
			RpcStartAnimationLookUp ();
		}

		[ClientRpc]
		void RpcStartAnimationLookUp () {
			anim.SetBool ("looking", true);
		}

		public void HideDices () {
			if (!isLocalPlayer) {
				return;
			}
				
			lookBtn.SetActive (true);
			hideBtn.SetActive (false);
			anim.SetBool ("looking", false);
		}

		[Command]
		void CmdStartAnimationHide () {
			RpcStartAnimationHide ();
		}

		[ClientRpc]
		void RpcStartAnimationHide () {
			anim.SetBool ("looking", false);
		}

		void OnDestroy () {
			ClickManager.OnLook -= LookUpDices;
			ClickManager.OnHide -= HideDices;
		}

		void OnGUI () {
			/*GUIStyle s = new GUIStyle ();
			s.fontSize = 30;
			s.fontStyle = FontStyle.Bold;
			s.normal.textColor = Color.white;

			GUI.color = Color.white;
			GUI.Label (rect1, gamePlayer.playerName + " -> Dices: " + availableDices.ToString (), s);

			if (dices.Length == 0) {
				return;
			}

			Rect tmpRect = rect2;

			GUI.Label (tmpRect, dices [0].ToString (), s);
			tmpRect.x += 40;
			GUI.Label (tmpRect, dices [1].ToString (), s);
			tmpRect.x += 40;
			GUI.Label (tmpRect, dices [2].ToString (), s);
			tmpRect.x += 40;
			GUI.Label (tmpRect, dices [3].ToString (), s);
			tmpRect.x += 40;
			GUI.Label (tmpRect, dices [4].ToString (), s);
*/
		}

		public void UpdateDieFaceValueDisplay () {
			rect1 = new Rect (250, gamePlayer.slotId.GetIndex () * 30 + 200, 320, 100);
			rect2 = new Rect (20, gamePlayer.slotId.GetIndex () * 30 + 200, 300, 100);
			//dices = spawnManager.GetDiceValues ();
		}

	}

}