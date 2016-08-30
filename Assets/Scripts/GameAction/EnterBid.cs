using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class EnterBid : IAction {

	Bid bid;

	public EnterBid (Bid bid) {
		this.bid = bid;
	}

	#region IAction implementation

	public void ExecuteAction () {
		//Bid tableBid = Table.singleton.bid;
		//if (tableBid.Equals (null)) {
				
		//}
	}

	#endregion

}