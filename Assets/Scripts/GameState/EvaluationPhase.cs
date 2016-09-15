using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MW_DiceGame;

public class EvaluationPhase : AbstractState {

	Table table;

	public EvaluationPhase (Table table) {
		Debug.Log ("EvaluationPhase");
		this.table = table;
		this.action = null;
	}


	public override void OnEnter () {
		Debug.Log ("OnEnter - CountDicesOnTable");
		CountDicesOnTable (table.players);


		table.RpcLookUpAllDices ();
		//Debug.Log ("OnEnter - LookUpAllDices");

	}

	public override void Execute () {
		Debug.Log ("Execute");

		//table.RpcEnablePlayerCams ();

		if (action != null) {
			action.ExecuteAction (table);
		}
	}

	public override void OnExit () {
		Debug.Log ("OnExit");
		//table.RpcDisablePlayerCams ();
		//action.ExecuteAction (table);
	}

	public override void EnterBidding () {
		table.SetGameState (table.bidding);
	}

	public override void LeaveGame () {
		table.SetGameState (table.gameOver);
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