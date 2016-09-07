using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class EvaluationPhase : AbstractState {

	Table table;

	public EvaluationPhase (Table table, IAction action) {
		Debug.Log ("EvaluationPhase");
		this.table = table;
		this.action = action;
	}


	public override void Execute () {
		// Disable Control Buttons


		table.CountDicesOnTable ();
		table.LookUpAllDices ();

		action.ExecuteAction (table);
	}

	public override void EnterBidding () {
		table.SetGameState (new Bidding (table, action));
	}

	public override void LeaveGame () {
		table.SetGameState (new GameOver (table));
	}

}