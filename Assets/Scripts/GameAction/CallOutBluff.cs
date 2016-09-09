using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using MW_DiceGame;

public class CallOutBluff : IAction {
		
	#region IAction implementation

	public void ExecuteAction (Table table) {
		Debug.Log ("OnEnter - Evaluate CallOutBluff");

		int value = 0;
		table.dieFaceMap.TryGetValue (table.currentBid.dieFace, out value);
		Bid realBidOnTable = new Bid (table.currentBid.dieFace, value);

		Transform player;

		if (table.currentBid.IsBluff (realBidOnTable)) {
			Debug.Log ("Bluff: last player loses a dice.");
			player = table.GetLastPlayer ();
		} else {
			Debug.Log ("No Bluff: current player " + table.GetCurrentPlayer ().GetComponent<GamePlayer> ().playerName + " loses a dice.");
			player = table.GetCurrentPlayer ();
		}

		DiceCup diceCup = player.GetComponent<DiceCup> ();
		diceCup.CmdDecreaseDiceFromPlayer ();
	}

	#endregion

}