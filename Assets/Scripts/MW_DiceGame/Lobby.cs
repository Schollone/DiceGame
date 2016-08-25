using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace MW_DiceGame {

	public class Lobby : NetworkLobbyManager {

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