using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using MW_DiceGame;
using Prototype.NetworkLobby;

public class EventManager : MonoBehaviour {

	public delegate void ClickAction ();

	public static event ClickAction OnLook;
	public static event ClickAction OnHide;
	public static event ClickAction OnDeclareSpotOn;
	public static event ClickAction OnCallOutBluff;

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
		SendAction (ActionMsg.EnterBid);
		//SendDeclareBidSpotOnAction ();
		//if (OnDeclareSpotOn != null) {
		//	OnDeclareSpotOn ();
		//}
	}

	public void CallOutBluff () {
		SendAction (ActionMsg.CallOutBluff);
		//SendCallOutBluffAction ();
		//if (OnCallOutBluff != null) {
		//	OnCallOutBluff ();
		//}
	}

	public void Pause () {
		NetworkClient client = LobbyManager.singleton.client;
		if (client == null || !client.isConnected) {
			return;
		}

		var msg = new EmptyMessage ();
		client.Send (MsgType.LobbyReturnToLobby, msg);
	}

	public void OpenPauseMenu () {
		var pausemenu = GameObject.Find ("PauseMenu");
		pausemenu.GetComponent<Canvas> ().enabled = true;
	}

	public void ResumeGame () {
		var pausemenu = GameObject.Find ("PauseMenu");
		pausemenu.GetComponent<Canvas> ().enabled = false;
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