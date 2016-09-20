using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MW_DiceGame;

public class EnterBid : IAction {

	Bid bid;

	public EnterBid (Bid bid) {
		this.bid = bid;
	}

	#region IAction implementation

	public void ExecuteAction (Table table) {
		if (table.currentBid.Exists ()) {
			if (table.currentBid.CanBeReplacedWith (bid)) {
				UpdateBid (table, bid);
			}
		} else {
			UpdateBid (table, bid);
		}
	}

	#endregion

	void UpdateBid (Table table, Bid bid) {
		table.currentBid = bid;
		table.ChangeBidOnClients (bid);
	}

}