using UnityEngine;
using System.Collections;

public interface IState {

	void StartGame ();

	void NextPlayer ();

	void EnterEvaluationPhase ();

	void EnterBidding ();

	void LeaveGame ();
}