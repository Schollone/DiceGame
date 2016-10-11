using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using MW_DiceGame;

public class CallOutBluff : IAction {

	#region IAction implementation

	public void ExecuteAction (Table table) {
		int value = 0;
		table.dieFaceMap.TryGetValue (table.currentBid.dieFace, out value);
		Bid realBidOnTable = new Bid (table.currentBid.dieFace, value);

		Transform player;

		if (table.currentBid.IsBluff (realBidOnTable)) {
			player = table.GetPrevPlayer ();
			Debug.Log ("Bluff: last player, " + player.GetComponent<GamePlayer> ().playerName + ", loses a dice.");
		} else {
			player = table.GetCurrentPlayer ();
			Debug.Log ("No Bluff: current player " + player.GetComponent<GamePlayer> ().playerName + " loses a dice.");
		}

		DiceCup diceCup = player.GetComponent<DiceCup> ();
		diceCup.CmdDecreaseDiceFromPlayer ();
	}

	#endregion

}