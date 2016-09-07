using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class GameOver : AbstractState {

	Table table;

	public GameOver (Table table) {
		Debug.Log ("GameOver");
		this.table = table;
	}


	public override void Execute () {
		
	}

}

