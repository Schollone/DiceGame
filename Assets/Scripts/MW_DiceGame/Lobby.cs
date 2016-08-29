using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace MW_DiceGame {

	public class Lobby : NetworkLobbyManager {

		public override GameObject OnLobbyServerCreateGamePlayer (NetworkConnection conn, short playerControllerId) {
			Debug.Log ("Lobby - OnLobbyServerCreateGamePlayer");

			Transform startPos = GetStartPosition ();
			Transform[] startPosArray = startPositions.ToArray ();

			for (int i = 0; i < startPosArray.Length; i++) {
				startPos = startPosArray [i];
				SpawnPosition spawnPosition = startPos.GetComponent<SpawnPosition> ();
				Debug.Log ("IsTaken: " + spawnPosition.IsTaken);
				if (!spawnPosition.IsTaken) {
					Debug.Log ("IsNotTaken: " + spawnPosition.name);
					spawnPosition.IsTaken = true;
					break;
				}
			}
				
			GameObject go = (GameObject)Instantiate (gamePlayerPrefab, startPos.position, startPos.rotation);

			return go;
		}

		public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer) {

			Debug.Log ("OnLobbyServerSceneLoadedForPlayer");

			LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer> ();
			GamePlayer gp = gamePlayer.GetComponent<GamePlayer> ();

			gp.slotId = lp.slotId;
			gp.playerName = lp.playerName;
			gp.color = lp.color;

			return true;
		}

	}

}