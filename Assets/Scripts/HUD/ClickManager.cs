using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using MW_DiceGame;
using Prototype.NetworkLobby;

public class ClickManager : MonoBehaviour {

	public delegate void ClickAction ();

	public static event ClickAction OnLook;
	public static event ClickAction OnHide;

	public void Look () {
		if (OnLook != null) {
			OnLook ();
		}
	}

	public void Hide () {
		if (OnHide != null) {
			OnHide ();
		}
	}

	public void DeclareBidSpotOn () {
		SendAction (ActionMsg.DeclareBidSpotOn);
	}

	public void CallOutBluff () {
		SendAction (ActionMsg.CallOutBluff);
	}

	public void OpenPauseMenu () {
		var lobby = LobbyManager.singleton as LobbyManager;
		lobby.pausePanel.gameObject.SetActive (true);
	}

	public void SendAction (short action) {
		NetworkClient client = LobbyManager.singleton.client;
		if (client == null || !client.isConnected) {
			return;
		}

		var msg = new ActionMessage ();
		msg.connectionId = client.connection.connectionId;
		client.Send (action, msg);
	}

}