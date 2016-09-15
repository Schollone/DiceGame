using UnityEngine;
using System.Collections;
using MW_DiceGame;
using UnityEngine.Networking;

public class Bidding : AbstractState {

	Table table;

	public Bidding (Table table) {
		Debug.Log ("Bidding");
		this.table = table;
		this.action = null;
	}


	public override void OnEnter () {
		Debug.LogWarning ("OnEnter - Reset Current Bid");
		table.ResetBid ();

		Debug.Log ("OnEnter - UnlockControlButtons");
		//SendToClients (ActionMsg.UnlockControlButtons);
		table.SendUnlockControlsEvent ();

		Debug.Log ("OnEnter - ThrowDices");
		ThrowDices (table.players);

		Debug.Log ("OnEnter - SetPlayerToBegin");
		SetPlayerToBegin ();
	}

	public override void Execute () {
		Debug.Log ("Execute");

		if (action != null) {
			action.ExecuteAction (table);
		}
	}

	public override void OnExit () {
		Debug.Log ("OnExit - LockControlButtons");
		//SendToClients (ActionMsg.LockControlButtons);
		table.SendLockControlsEvent ();

		Debug.Log ("OnExit - HideAllDices");
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

	/*void SendToClients (short msgType) {
		Debug.Log ("SendToClients");
		ActionMessage msg = new ActionMessage ();
		NetworkServer.SendToAll (msgType, msg);
	}*/

	void ThrowDices (Transform players) {
		Debug.Log ("ThrowDices " + players.childCount);
		for (int i = 0; i < players.childCount; i++) {
			Transform player = players.GetChild (i);
			player.GetComponent<DiceCup> ().CmdFillDiceCupWithDices ();
		}
	}
}