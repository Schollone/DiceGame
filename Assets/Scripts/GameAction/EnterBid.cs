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
				table.currentBid = bid;
				table.ChangeBidOnClients (bid);
				//SendToClients ();
			}
		} else {
			table.currentBid = bid;
			table.ChangeBidOnClients (bid);
			//SendToClients ();
		}
	}

	#endregion

	void SendToClients () {
		Debug.Log ("Bestätige den Clients das neue Gebot");
		/*ActionMessage msg = new ActionMessage ();
		msg.bid = bid;
		NetworkServer.SendToAll (ActionMsg.EnterBid, msg);*/
	}

}