using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class AnimationMethods : MonoBehaviour {

	public void StartEvalation () {
		GamePlayer gp = transform.parent.GetComponent<GamePlayer> ();

		if (gp.isLocalPlayer) {
			Table.singleton.ExecuteState ();
		}
	}

	public void EnableEvaluationCams () {
		GamePlayer gp = transform.parent.GetComponent<GamePlayer> ();

		if (gp.isLocalPlayer) {
			Table.singleton.EnablePlayerCams ();
		}
	}

	public void DisableEvaluationCams () {
		GamePlayer gp = transform.parent.GetComponent<GamePlayer> ();

		if (gp.isLocalPlayer) {
			Table.singleton.DisablePlayerCams ();
		}
	}
}
