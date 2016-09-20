using UnityEngine;
using System.Collections;
using MW_DiceGame;
using UnityEngine.Networking;

public class Bidding : AbstractState {

	Table table;

	public Bidding (Table table) {
		this.table = table;
		this.action = null;
	}


	public override void OnEnter () {
		table.ResetBid ();

		table.SendUnlockControlsEvent ();

		ThrowDices (table.players);

		SetPlayerToBegin ();
	}

	public override void Execute () {
		if (action != null) {
			action.ExecuteAction (table);
		}
	}

	public override void OnExit () {
		table.SendLockControlsEvent ();
		table.RpcHideAllDices ();
	}

	public override void NextPlayer () {
		table.NextPlayerHisTurn ();
	}

	public override void EnterEvaluationPhase () {
		table.SetGameState (table.evaluationPhase);
	}





	void SetPlayerToBegin () {
		var player = table.GetCurrentPlayer ();
		player.GetComponent<GamePlayer> ().CmdItIsYourTurn (true);
	}

	void ThrowDices (Transform players) {
		for (int i = 0; i < players.childCount; i++) {
			Transform player = players.GetChild (i);
			player.GetComponent<DiceCup> ().CmdFillDiceCupWithDices ();
		}
	}
}