using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class GameOver : IState {

	Table table;

	public GameOver (Table table) {
		Debug.Log ("GameOver");
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
	}

	public void LeaveGame () {
	}

	#endregion
}

