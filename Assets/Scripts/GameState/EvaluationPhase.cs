using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class EvaluationPhase : IState {

	Table table;

	public EvaluationPhase (Table table) {
		Debug.Log ("EvaluationPhase");
		this.table = table;
	}


	#region IState implementation

	public void StartGame () {
	}

	public void NextPlayer () {
	}

	public void EnterEvaluationPhase () {
	}

	public void EnterBidding () {
		table.SetGameState (new Bidding (table));
	}

	public void LeaveGame () {
		table.SetGameState (new GameOver (table));
	}

	#endregion
}

