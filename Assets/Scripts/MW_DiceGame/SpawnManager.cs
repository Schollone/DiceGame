using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace MW_DiceGame {

	public struct DiceStruct {
		public GameObject dice;
	}

	public class SyncListDice : SyncListStruct<DiceStruct> {

	}

	public class SpawnManager : NetworkBehaviour {

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


		public override void OnStartClient () {
			Debug.Log ("SpawnManager - OnStartClient");
			base.OnStartClient ();

			assetId = prefab.GetComponent<NetworkIdentity> ().assetId;
				

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





		/*void HandleSyncListChanged (SyncList<DiceStruct>.Operation op, int itemIndex) {
			Debug.Log ("HandleSyncListChanged ---------------------------------------------------------------");
		}

		void HandleEventClientsAreReady () {
			Debug.LogWarningFormat ("SpawnManager - EventClientsAreReady - {0}, {1}", isServer, isClient);

		}*/

		public GameObject SpawnHandler (Vector3 position, NetworkHash128 assedId) {
			return GetFromPool (position);
		}

		public void UnSpawnHandler (GameObject spawned) {
			PutBackToPool (spawned);
		}

		public GameObject GetFromPool (Vector3 position) {
			foreach (DiceStruct ds in dices) {
				if (ds.dice != null && !ds.dice.activeInHierarchy) {
					Debug.Log ("Activating object " + ds.dice.name + " at " + position);
					ds.dice.transform.position = position;
					ds.dice.transform.SetAsFirstSibling ();
					ds.dice.SetActive (true);
					return ds.dice;
				}
			}
			//foreach (GameObject dice in pool) {
			//if (!dice.activeInHierarchy) {
			//Debug.Log ("Activating object " + dice.name + " at " + position);
			//dice.transform.position = position;
			//dice.transform.SetAsFirstSibling ();
			//dice.SetActive (true);
			//return dice;
			//}
			//}
			Debug.LogError ("Could not grab object from pool, nothing available");
			return null;
		}

		public void PutBackToPool (GameObject spawned) {
			if (spawned.activeInHierarchy) {
				spawned.transform.SetAsLastSibling ();
				spawned.GetComponent<Dice> ().ResetDice ();
				spawned.SetActive (false);
			}

			//Debug.Log ("Re-pooling object " + spawned.name);
		}

		public void UnspawnAllObjects () {
			for (int i = 0; i < objectPoolSize; i++) {
				GameObject go = dices.GetItem (0).dice;
				//GameObject go = pool [i];
				UnSpawnHandler (go);
			}
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

			for (int i = 0; i < result.Length; i++) {
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