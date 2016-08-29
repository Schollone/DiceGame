using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace MW_DiceGame {

	public struct DiceStruct {
		public GameObject dice;

		public DiceStruct (GameObject dice) {
			this.dice = dice;
		}
	}

	public class SyncListDice : SyncListStruct<DiceStruct> {
		
	}

	public class SpawnManager : NetworkBehaviour {

		[SyncVar]
		public SyncListDice dices = new SyncListDice ();

		public int objectPoolSize = 5;
		public GameObject prefab;

		//[SyncVar (hook = "OnPoolUpdate")]
		//public GameObject[] pool;

		public NetworkHash128 assetId { get; set; }

		public delegate GameObject SpawnDelegate (Vector3 position, NetworkHash128 assedId);

		public delegate void UnSpawnDelegate (GameObject spawned);


		public GameObject diceContainer;
		string folderName = "PoolObjects";


		void Awake () {
			dices.Callback = HandleSyncListChanged;
		}

		public override void OnStartLocalPlayer () {
			Debug.Log ("SpawnManager - OnStartServer");
			base.OnStartClient ();

			assetId = prefab.GetComponent<NetworkIdentity> ().assetId;
			ClientScene.RegisterSpawnHandler (assetId, SpawnHandler, UnSpawnHandler);

			CmdAddDices ();
		}

		[Command]
		public void CmdAddDices () {
			GamePlayer gp = GetComponent<GamePlayer> ();

			folderName = gp.playerName;
			Debug.LogFormat ("SpawnManager - Create {0}", folderName);

			for (int i = 0; i < objectPoolSize; i++) {
				GameObject diceGO = (GameObject)Instantiate (prefab, Vector3.zero, Quaternion.identity);
				diceGO.name = "Dice_" + folderName + "_" + i;
				//diceGO.SetActive (false);
				diceGO.GetComponent<MeshRenderer> ().enabled = false;

				Dice dice = diceGO.GetComponent<Dice> ();
				dice.diceCupId = netId;
				dice.color = gp.color;

				PutInDiceContainer (diceGO);

				DiceStruct ds = new DiceStruct (diceGO);

				//pool [i] = dice;
				Debug.Log ("Add " + diceGO);
				//dices.Add (ds);
				dices.Add (ds);
			}
		}

		void HandleSyncListChanged (SyncList<DiceStruct>.Operation op, int itemIndex) {
			Debug.Log ("HandleSyncListChanged --------------------------------------------------------------- " + itemIndex);
			Debug.Log (dices.Count);
			Debug.Log (dices.GetItem (itemIndex).dice);
			foreach (DiceStruct ds in dices) {
				GameObject go = ds.dice;
				Debug.LogWarning ("in Dices " + folderName + ": " + go);
			}
		}

		/*public override void OnStartLocalPlayer () {
			Debug.LogError ("SpawnManager - OnStartLocalPlayer");
			base.OnStartLocalPlayer ();

			folderName = GetComponent<GamePlayer> ().playerName;
			Debug.LogFormat ("SpawnManager - Create {0}", folderName);

			for (int i = 0; i < objectPoolSize; i++) {
				GameObject dice = (GameObject)Instantiate (prefab, Vector3.zero, Quaternion.identity);
				dice.name = "Dice_" + folderName + "_" + i;
				dice.SetActive (false);
				PutInDiceContainer (dice);

				DiceStruct ds = new DiceStruct ();
				ds.dice = dice;

				//pool [i] = dice;
				Debug.Log ("Add " + dice);
				dices.Add (ds);
			}

			ClientScene.RegisterSpawnHandler (assetId, SpawnHandler, UnSpawnHandler);
		}*/


		public GameObject SpawnHandler (Vector3 position, NetworkHash128 assedId) {
			return GetFromPool (position);
		}

		public void UnSpawnHandler (GameObject spawned) {
			Debug.Log ("-------------------------------------------------- SpawnManager old");
			PutBackToPool (spawned);
		}

		public GameObject GetFromPool (Vector3 position) {
			Debug.LogWarning ("GetFromPool " + position + " - dices.Count=" + dices.Count);

			DiceStruct[] dsArray = new DiceStruct[dices.Count];
			dices.CopyTo (dsArray, 0);

			for (int i = 0; i < dsArray.Length; i++) {
				DiceStruct ds = dsArray [i];

				GameObject diceGo = ds.dice;
				Debug.Log ("Dice: " + diceGo);

				if (diceGo != null && !diceGo.GetComponent<MeshRenderer> ().enabled) {
					Debug.Log ("Activating object " + diceGo.name + " at " + position);
					diceGo.transform.position = position;
					//diceGo.transform.SetAsFirstSibling ();

					//diceGo.SetActive (true);
					diceGo.GetComponent<MeshRenderer> ().enabled = true;

					return diceGo;
				}


				

				//if (diceGo != null && !diceGo.activeInHierarchy) {

			}
			Debug.LogError ("Could not grab object from pool, nothing available");
			return null;
		}

		void PutBackToPool (GameObject spawned) {
			Debug.Log ("PutBackToPool " + spawned);
			//if (spawned.activeInHierarchy) {
			//spawned.transform.SetAsLastSibling ();
			spawned.GetComponent<Dice> ().ResetDice ();
			spawned.GetComponent<MeshRenderer> ().enabled = false;
			//spawned.SetActive (false);
			//}

			//Debug.Log ("Re-pooling object " + spawned.name);
		}

		public void UnspawnAllObjects () {
			foreach (DiceStruct ds in dices) {

				GameObject go = ds.dice;
				//UnSpawnHandler (go);
				
				//Debug.Log ("ds.active: " + ds.active);
				//if (ds.active) {

				Debug.Log ("Unspawn: " + go);
				NetworkServer.UnSpawn (go);
				//}
			}

			/*for (int i = 0; i < objectPoolSize; i++) {
				GameObject go = dices.GetItem (0).dice;
				//GameObject go = pool [i];
				UnSpawnHandler (go);
			}*/
		}

		/*void OnDestroy () {
			Debug.Log ("On Destroy");
			var diceContainer = GameObject.Find (folderName);
			GameObject.Destroy (diceContainer);
		}*/

		private void PutInDiceContainer (GameObject dice) {
			diceContainer = GameObject.Find (folderName);
			if (diceContainer == null) {
				diceContainer = new GameObject (folderName);
			}
			dice.transform.parent = diceContainer.transform;
		}

		public int[] GetDiceValues () {
			//int[] result = new int[pool.Length];
			int[] result = new int[dices.Count];

			for (int i = 0; i < dices.Count; i++) {
				DiceStruct ds = dices.GetItem (i);
				GameObject diceGo = ds.dice;
				//GameObject diceGo = pool [i];

				if (diceGo == null) {
					continue;
				}

				if (diceGo.activeInHierarchy) {
					Dice dice = diceGo.GetComponent<Dice> ();
					result [i] = dice.dieFace.GetIndex ();
				}
			}

			return result;
		}

	}

}