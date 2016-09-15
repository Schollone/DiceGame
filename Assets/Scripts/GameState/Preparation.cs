using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class Preparation : AbstractState {

	Table table;

	public Preparation (Table table) {
		Debug.Log ("Preparation");
		this.table = table;
		this.action = null;
	}

	public override void OnEnter () {

	}

	public override void Execute (IAction action = null) {

	}

	public override void OnExit () {
		
	}


	public override void StartGame () {
		table.SetGameState (table.bidding);
	}

}

