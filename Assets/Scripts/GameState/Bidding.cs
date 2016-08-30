using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class Bidding : IState {

	Table table;

	public Bidding (Table table) {
		Debug.Log ("Bidding");
		this.table = table;
	}


	#region IState implementation

	public void StartGame () {
	}

	public void NextPlayer () {
		table.SetGameState (new Bidding (table));
	}

	public void EnterEvaluationPhase () {
		table.SetGameState (new EvaluationPhase (table));
	}

	public void EnterBidding () {
	}

	public void LeaveGame () {
	}

	#endregion
}

