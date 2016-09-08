using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Prototype.NetworkLobby;
using MW_DiceGame;

public class Lobby : LobbyHook {

	public override GameObject OnLobbyServerCreateGamePlayer (NetworkManager manager, NetworkConnection conn, short playerControllerId) {
		//Debug.Log ("Lobby - OnLobbyServerCreateGamePlayer");
		var lobbyManager = manager as LobbyManager;

		Transform startPos = lobbyManager.GetStartPosition ();
		Transform[] startPosArray = lobbyManager.startPositions.ToArray ();

		for (int i = 0; i < startPosArray.Length; i++) {
			startPos = startPosArray [i];
			SpawnPosition spawnPosition = startPos.GetComponent<SpawnPosition> ();
			//Debug.Log ("IsTaken: " + spawnPosition.IsTaken);
			if (!spawnPosition.IsTaken) {
				//Debug.Log ("IsNotTaken: " + spawnPosition.name);
				spawnPosition.IsTaken = true;
				break;
			}
		}
				
		GameObject go = (GameObject)Instantiate (lobbyManager.gamePlayerPrefab, startPos.position, startPos.rotation);

		return go;
	}

	public override bool OnLobbyServerSceneLoadedForPlayer (NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {

		//Debug.Log ("OnLobbyServerSceneLoadedForPlayer");

		Prototype.NetworkLobby.LobbyPlayer lp = lobbyPlayer.GetComponent<Prototype.NetworkLobby.LobbyPlayer> ();
		GamePlayer gp = gamePlayer.GetComponent<GamePlayer> ();

		gp.slotId = lp.slotId;
		gp.playerName = lp.playerName;
		gp.color = lp.color;

		return true;
	}

}