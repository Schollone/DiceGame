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


	public override void OnEnter () {
		Debug.Log ("OnEnter - CountDicesOnTable");
		CountDicesOnTable (table.players);

		Debug.Log ("OnEnter - LookUpAllDices");
		table.LookUpAllDices ();

		//action.ExecuteAction (table);
	}

	public override void Execute () {
		Debug.Log ("Execute");
		action.ExecuteAction (table);
	}

	public override void OnExit () {
		Debug.Log ("OnExit");
		//action.ExecuteAction (table);
	}

	public override void EnterBidding () {
		table.SetGameState (new Bidding (table, action));
	}

	public override void LeaveGame () {
		table.SetGameState (new GameOver (table));
	}






	void CountDicesOnTable (Transform players) {
		table.dieFaceMap.Clear ();

		for (int i = 0; i < players.childCount; i++) {
			Transform player = players.GetChild (i);
			SpawnManager spawnManager = player.GetComponent<SpawnManager> ();
			DieFaces[] dieFaces = spawnManager.GetDieFacesFromPlayer ();
			for (int j = 0; j < dieFaces.Length; j++) {

				DieFaces dieFace = dieFaces [j];
				int value = 1;

				if (table.dieFaceMap.ContainsKey (dieFace)) {
					value = table.dieFaceMap [dieFace];
					value++;
					table.dieFaceMap [dieFace] = value;
					//dieFaceMap.Add (dieFace, value + 1);
				} else {
					table.dieFaceMap.Add (dieFace, value);
				}

			}

		}
	}

}