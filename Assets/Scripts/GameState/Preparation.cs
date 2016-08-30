using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class Preparation : IState {

	Table table;

	public Preparation (Table table) {
		Debug.Log ("Preparation");
		this.table = table;

		this.table.InitPlayers ();
	}


	#region IState implementation

	public void StartGame () {

		table.ThrowDices (); // add callback for when all dices are thrown

		//table.CountDicesOnTable ();

		table.SetGameState (new Bidding (table));
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

