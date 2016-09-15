using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class AnimationMethods : MonoBehaviour {

	/*public void OnAnimationPlayed () {
		//Invoke ("StartEvalation", 2f);
		//Invoke ("StopEvalation", 7f);
	}*/

	public void StartEvalation () {
		Debug.LogWarning ("StartEvaluation!!!!!!!!!!!!!!!!!!!!!!!!!! " + Time.fixedTime);
		Table.singleton.ExecuteState ();
	}

	/*public void StopEvalation () {
		Debug.LogWarning ("StopEvaluation!!!!!!!!!!!!!!!!!!!!!!!!!! " + Time.fixedTime);
		Table.singleton.DisablePlayerCams ();
	}*/


	public void EnableEvaluationCams () {
		Table.singleton.EnablePlayerCams ();
	}

	public void DisableEvaluationCams () {
		Table.singleton.DisablePlayerCams ();
	}
}
