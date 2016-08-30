using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using MW_DiceGame;

public class CallOutBluff : IAction {
		
	#region IAction implementation

	public void ExecuteAction () {
		//Table.singleton.NextPlayer (); // Instance nur auf dem Server verfügbar!
		/*
		NetworkClient client = Lobby.singleton.client;
		if (client == null || !client.isConnected) {
			return;
		}

		var msg = new EmptyMessage ();
		client.Send (ActionMsg.CallOutBluff, msg);*/
	}

	#endregion

}