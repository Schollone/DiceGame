using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class AnimationMethods : MonoBehaviour {

	/*public void OnAnimationPlayed () {
		//Invoke ("StartEvalation", 2f);
		//Invoke ("StopEvalation", 7f);
	}*/

	public void StartEvalation () {
		GamePlayer gp = transform.parent.GetComponent<GamePlayer> ();

		if (gp.isLocalPlayer) {
			Debug.LogWarning ("StartEvaluation!!!!!!!!!!!!!!!!!!!!!!!!!! " + Time.fixedTime);
			Table.singleton.ExecuteState ();
		}
	}

	/*public void StopEvalation () {
		Debug.LogWarning ("StopEvaluation!!!!!!!!!!!!!!!!!!!!!!!!!! " + Time.fixedTime);
		Table.singleton.DisablePlayerCams ();
	}*/


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
