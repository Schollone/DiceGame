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

	void Awake () {
		GamePlayer.ShowControlsEvent += OnShowControls;
	}

	void OnShowControls (bool show) {
		Debug.Log ("ShowControlsEvent - " + show);

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

