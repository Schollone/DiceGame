using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace MW_DiceGame {

	public class SpawnManager2 : NetworkBehaviour {

		public GamePlayer gamePlayer;
		public GameObject spawnablePrefab;
		public int poolSize = 5;

		public GameObject container;


		public NetworkHash128 assetId { get; set; }

		public delegate GameObject SpawnDelegate (Vector3 position, NetworkHash128 assedId);

		public delegate void UnSpawnDelegate (GameObject spawned);




		void Start () {
			
		}

		public override void OnStartClient () {
			base.OnStartClient ();

			string folderName = gamePlayer.playerName;
			container = GetContainer (folderName);

			assetId = spawnablePrefab.GetComponent<NetworkIdentity> ().assetId;
			ClientScene.RegisterSpawnHandler (assetId, SpawnHandler, UnSpawnHandler);
		}

		public override void OnStartLocalPlayer () {
			base.OnStartLocalPlayer ();



			CmdSpawnDices ();
		}

		GameObject SpawnHandler (Vector3 position, NetworkHash128 assedId) {
			return GetDice ();
		}

		GameObject GetDice () {
			GameObject diceGO = (GameObject)Instantiate (spawnablePrefab, Vector3.zero, Quaternion.identity);
			diceGO.name = "Dice_" + container.name;

			Dice dice = diceGO.GetComponent<Dice> ();
			dice.diceCupId = netId;
			dice.color = gamePlayer.color;

			diceGO.transform.SetParent (container.transform);

			return diceGO;
		}

		public void UnSpawnHandler (GameObject spawned) {
			Debug.LogWarning ("!!!!!!!!!!!!!!!!!!! UnSpawnHandler=" + spawned);
			Destroy (spawned);
		}

		GameObject GetContainer (string folderName) {
			GameObject container = GameObject.Find (folderName);
			if (container == null) {
				container = new GameObject (folderName);
			}

			return container;
		}

		[Command]
		void CmdSpawnDices () {
			for (int i = 0; i < poolSize; i++) {
				/*GameObject diceGO = (GameObject)Instantiate (spawnablePrefab, Vector3.zero, Quaternion.identity);
				diceGO.name = "Dice_" + container.name + "_" + i;

				Dice dice = diceGO.GetComponent<Dice> ();
				dice.diceCupId = netId;
				dice.color = gamePlayer.color;
				//dice.visibility = false;

				diceGO.transform.SetParent (container.transform);*/

				var diceGO = GetDice ();
				NetworkServer.Spawn (diceGO);
			}
		}


		public int[] GetDiceValues () {
			//int[] result = new int[pool.Length];
			int count = container.transform.childCount;
			int[] result = new int[poolSize];

			for (int i = 0; i < count; i++) {
				Transform diceGO = container.transform.GetChild (i);

				Dice dice = diceGO.GetComponent<Dice> ();
				result [i] = dice.dieFace.GetIndex ();

				//DiceStruct ds = dices.GetItem (i);
				//GameObject diceGo = ds.dice;
				//GameObject diceGo = pool [i];

				/*if (diceGO == null) {
					continue;
				}*/

				/*if (diceGO.activeInHierarchy) {
					Dice dice = diceGo.GetComponent<Dice> ();
					result [i] = dice.dieFace.GetIndex ();
				}*/
			}

			return result;
		}

	}

}