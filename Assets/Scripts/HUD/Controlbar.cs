using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MW_DiceGame;

public class Controlbar : MonoBehaviour {

	public GameObject bidController;
	public Button enterBidButton;
	public Button callOutBluffButton;
	public Button declareBidSpotOnButton;
	public Button lookUpDicesButton;
	public Button hideDicesButton;

	Button[] bidControllerButtons;

	void Awake () {
		GamePlayer.ShowControlsEvent += OnShowControls;

		bidControllerButtons = bidController.GetComponentsInChildren<Button> ();
	}

	void OnShowControls (bool show) {
		Debug.Log ("ShowControlsEvent - " + show);

		for (int i = 0; i < bidControllerButtons.Length; i++) {
			Button bidControllerButton = bidControllerButtons [i];
			bidControllerButton.interactable = show;
		}

		enterBidButton.interactable = show;
		callOutBluffButton.interactable = show;
		declareBidSpotOnButton.interactable = show;
		lookUpDicesButton.interactable = show;
		hideDicesButton.interactable = show;
	}

	void OnDestroy () {
		GamePlayer.ShowControlsEvent -= OnShowControls;
	}

}

