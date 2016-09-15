using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class GameOver : AbstractState {

	Table table;

	public GameOver (Table table) {
		Debug.Log ("GameOver");
		this.table = table;
		this.action = null;
	}


	public override void OnEnter () {
		
	}

	public override void Execute (IAction action = null) {
		
	}

	public override void OnExit () {
		
	}

}

