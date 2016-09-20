using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MW_DiceGame;

public class EvaluationPhase : AbstractState {

	Table table;

	public EvaluationPhase (Table table) {
		this.table = table;
		this.action = null;
	}


	public override void OnEnter () {
		CountDicesOnTable (table.players);

		table.RpcLookUpAllDices ();
	}

	public override void Execute () {
		if (action != null) {
			action.ExecuteAction (table);
		}
	}

	public override void OnExit () {
	}

	public override void EnterBidding () {
		table.SetGameState (table.bidding);
	}

	public override void LeaveGame () {
		table.SetGameState (table.gameOver);
	}






	void CountDicesOnTable (Transform players) {
		table.dieFaceMap.Clear ();

		loopPlayers (players);
	}

	void loopPlayers (Transform players) {
		for (int i = 0; i < players.childCount; i++) {
			Transform player = players.GetChild (i);
			SpawnManager spawnManager = player.GetComponent<SpawnManager> ();
			loopDieFaces (spawnManager);
		}
	}

	void loopDieFaces (SpawnManager spawnManager) {
		DieFaces[] dieFaces = spawnManager.GetDieFacesFromPlayer ();

		for (int j = 0; j < dieFaces.Length; j++) {
			DieFaces dieFace = dieFaces [j];

			if (table.dieFaceMap.ContainsKey (dieFace)) {
				IncreaseDieFaceCount (dieFace);
			} else {
				SetDieFaceCount (dieFace, 1);
			}

		}
	}

	void IncreaseDieFaceCount (DieFaces dieFace) {
		int value = table.dieFaceMap [dieFace];
		value++;
		table.dieFaceMap [dieFace] = value;
	}

	void SetDieFaceCount (DieFaces dieFace, int count) {
		table.dieFaceMap.Add (dieFace, count);
	}
}