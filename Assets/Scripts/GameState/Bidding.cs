using UnityEngine;
using System.Collections;
using MW_DiceGame;
using UnityEngine.Networking;

public class Bidding : AbstractState {

	Table table;

	public Bidding (Table table, IAction action) {
		Debug.Log ("Bidding");
		this.table = table;
		this.action = action;
	}


	public override void Execute () {
		table.ThrowDices ();

		Debug.LogWarning ("Reset Current Bid");
		table.currentBid = new Bid (DieFaces.Null, 0);
		// reset bid view ------------------------------------------

		table.NextPlayer ();
		//Transform player = table.GetCurrentPlayer ();
		//player.GetComponent<GamePlayer> ().CmdItIsYourTurn (true); // called twice
	}

	public override void StartGame () {
		//Transform player = table.GetCurrentPlayer ();
		//player.GetComponent<GamePlayer> ().CmdItIsYourTurn (true);
	}

	public override void NextPlayer () {
		//table.SetGameState (new Bidding (table));
		//action.ExecuteAction (table);
	}

	public override void EnterEvaluationPhase () {
		table.SetGameState (new EvaluationPhase (table, action));
	}

}

