﻿using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class DeclareBidSpotOn : IAction {
		
	#region IAction implementation

	public void ExecuteAction (Table table) {
		Debug.Log ("OnEnter - Evaluate DeclareBidSpotOn");

		int value = 0;
		table.dieFaceMap.TryGetValue (table.currentBid.dieFace, out value);
		Bid realBidOnTable = new Bid (table.currentBid.dieFace, value);

		if (table.currentBid.IsSpotOn (realBidOnTable)) {
			Debug.Log ("IsSpotOn: Everybody loses a dice!");
			table.DecreaseDiceFromOtherPlayers ();
		} else {
			Debug.Log ("Not Spot On: " + table.GetCurrentPlayer ().GetComponent<GamePlayer> ().playerName + " loses a dice");
			Transform player = table.GetCurrentPlayer ();
			DiceCup diceCup = player.GetComponent<DiceCup> ();
			diceCup.CmdDecreaseDiceFromPlayer ();
		}
	}

	#endregion

}