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
		table.ResetBid ();
		// reset bid view ------------------------------------------

		//table.NextPlayer ();
		var player = table.GetCurrentPlayer ();
		player.GetComponent<GamePlayer> ().CmdItIsYourTurn (true);
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

