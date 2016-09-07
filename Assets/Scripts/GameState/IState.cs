using UnityEngine;
using System.Collections;

public interface IState {

	IAction GetActionStrategy ();

	void SetActionStrategy (IAction action);

	void Execute ();

	void StartGame ();

	void NextPlayer ();

	void EnterEvaluationPhase ();

	void EnterBidding ();

	void LeaveGame ();
}