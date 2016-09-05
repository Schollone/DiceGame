using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



namespace Prototype.NetworkLobby {
	// Subclass this and redefine the function you want
	// then add it to the lobby prefab
	public abstract class LobbyHook : MonoBehaviour {
		public virtual bool OnLobbyServerSceneLoadedForPlayer (NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
			return false;
		}

		public virtual GameObject OnLobbyServerCreateGamePlayer (NetworkManager manager, NetworkConnection conn, short playerControllerId) {
			return null;
		}
	}

}
