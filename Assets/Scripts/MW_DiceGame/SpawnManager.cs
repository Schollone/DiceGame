using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;

namespace MW_DiceGame {

	public class SpawnManager : NetworkBehaviour {

		public GamePlayer gamePlayer;
		public GameObject spawnablePrefab;
		public GameObject unspawnParticleEffectPrefab;
		[SyncVar (hook = "OnReadyChange")]
		public bool ready;
		public int poolSize = 5;

		public GameObject container;


		public NetworkHash128 assetId { get; set; }

		public delegate GameObject SpawnDelegate (Vector3 position, NetworkHash128 assedId);

		public delegate void UnSpawnDelegate (GameObject spawned);


		public override void OnStartServer () {
			base.OnStartServer ();
			//Debug.LogWarning ("OnStartServer -- SpawnManager foldername = " + gamePlayer.playerName);

			/*string folderName = gamePlayer.playerName;
			container = GetContainer (folderName);

			CmdSpawnDices ();*/
		}

		public override void OnStartClient () {
			base.OnStartClient ();
			//Debug.Log ("OnStartClient -- SpawnManager");
			string folderName = gamePlayer.playerName;
			container = GetContainer (folderName);

			assetId = spawnablePrefab.GetComponent<NetworkIdentity> ().assetId;
			ClientScene.RegisterSpawnHandler (assetId, SpawnHandler, UnSpawnHandler);
		}

		public override void OnStartLocalPlayer () {
			base.OnStartLocalPlayer ();

			CmdSpawnDices ();

			CmdClientIsReady ();
			//if (!isServer) {

			//} else {
				
			//}
		}

		void Start () {
			//SendClientReady (ActionMsg.ClientReady);
		}

		[Command]
		void CmdClientIsReady () {
			this.ready = true;
		}


		void OnReadyChange (bool ready) {
			Debug.Log ("OnReadyChange " + ready);
			this.ready = ready;
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
				var diceGO = GetDice ();
				NetworkServer.Spawn (diceGO);
			}
		}

		void SendClientReady (short action) {
			NetworkClient client = LobbyManager.singleton.client;
			if (client == null || !client.isConnected) {
				return;
			}

			var msg = new ActionMessage ();
			msg.connectionId = client.connection.connectionId;
			if (isServer) {
				//Table.singleton.ServerIsReady ();
			} else {
				client.Send (action, msg);
			}
		}

		public DieFaces[] GetDieFacesFromPlayer () {
			int count = container.transform.childCount;
			DieFaces[] result = new DieFaces[count];

			for (int i = 0; i < count; i++) {
				Transform diceGO = container.transform.GetChild (i);

				Dice dice = diceGO.GetComponent<Dice> ();
				result [i] = dice.dieFace;
			}

			return result;
		}


		public int[] GetDiceValues () {
			int count = container.transform.childCount;
			int[] result = new int[poolSize];

			for (int i = 0; i < count; i++) {
				Transform diceGO = container.transform.GetChild (i);

				Dice dice = diceGO.GetComponent<Dice> ();
				result [i] = dice.dieFace.GetIndex ();
			}

			return result;
		}

	}

}