using UnityEngine;
using System.Collections;
using MW_DiceGame;

public class AnimationMethods : MonoBehaviour {

	public void OnAnimationPlayed () {
		Invoke ("StartEvalation", 2f);
		Invoke ("StopEvalation", 7f);
	}

	void StartEvalation () {
		Debug.LogWarning ("StartEvaluation!!!!!!!!!!!!!!!!!!!!!!!!!! " + Time.fixedTime);
		Table.singleton.ExecuteState ();
	}

	void StopEvalation () {
		Debug.LogWarning ("StopEvaluation!!!!!!!!!!!!!!!!!!!!!!!!!! " + Time.fixedTime);
		Table.singleton.RpcDisablePlayerCams ();
	}
}
