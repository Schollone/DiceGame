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
		if (table.BidAlreadyExists ()) {
			if (table.currentBid.CanBeReplacedWith (bid)) {
				table.currentBid = bid;
				SendToAll ();
			}
		} else {
			table.currentBid = bid;
			SendToAll ();
		}
	}

	#endregion

	void SendToAll () {
		Debug.Log ("SendToAll new Bid");
		ActionMessage msg = new ActionMessage ();
		msg.bid = bid;
		NetworkServer.SendToAll (ActionMsg.EnterBid, msg);
	}

}