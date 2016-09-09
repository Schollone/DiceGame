using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class Preparation : AbstractState {

	Table table;

	public Preparation (Table table) {
		Debug.Log ("Preparation");
		this.table = table;
	}

	public override void OnEnter () {
//		this.table.InitPlayers ();

		if (table.players.childCount == 1) {
			//Debug.Log ("Only Host is ready to fill up dice cups");
			//table.StartGame ();
		}
	}

	public override void Execute () {

	}

	public override void OnExit () {
		
	}


	public override void StartGame () {
		table.SetGameState (new Bidding (table, action));
	}

}

