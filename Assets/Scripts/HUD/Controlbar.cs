using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MW_DiceGame;

public class ControlBar : MonoBehaviour {

	public GameObject bidController;
	public Button enterBidButton;
	public Button callOutBluffButton;
	public Button declareBidSpotOnButton;
	public Button lookUpDicesButton;
	public Button hideDicesButton;

	Color enterBidTextColor;
	Color callOutBluffTextColor;
	Color declareBidSpotOnTextColor;
	Color lookUpDicesTextColor;
	Color hideDicesTextColor;

	float transparent = 0.5f;
	float full = 1f;

	void Awake () {
		GamePlayer.ShowControlsEvent += OnShowControls;
		GamePlayer.ActiveControlsEvent += OnActiveControls;

		enterBidTextColor = enterBidButton.transform.GetChild (0).GetComponent<Text> ().color;
		callOutBluffTextColor = callOutBluffButton.transform.GetChild (0).GetComponent<Text> ().color;
		declareBidSpotOnTextColor = declareBidSpotOnButton.transform.GetChild (0).GetComponent<Text> ().color;
		lookUpDicesTextColor = lookUpDicesButton.transform.GetChild (0).GetComponent<Text> ().color;
		hideDicesTextColor = hideDicesButton.transform.GetChild (0).GetComponent<Text> ().color;
	}

	void OnShowControls (bool show) {
		Debug.Log ("OnShowControls: " + show);
		enterBidButton.interactable = show;
		callOutBluffButton.interactable = show;
		declareBidSpotOnButton.interactable = show;
		lookUpDicesButton.interactable = show;
		hideDicesButton.interactable = show;

		if (show) {
			enterBidTextColor.a = full;
			callOutBluffTextColor.a = full;
			declareBidSpotOnTextColor.a = full;
			lookUpDicesTextColor.a = full;
			hideDicesTextColor.a = full;
		} else {
			enterBidTextColor.a = transparent;
			callOutBluffTextColor.a = transparent;
			declareBidSpotOnTextColor.a = transparent;
			lookUpDicesTextColor.a = transparent;
			hideDicesTextColor.a = transparent;
		}
	}

	void OnActiveControls (bool show) {
		Debug.Log ("OnActiveControls: " + show);
		enterBidButton.interactable = show;
		callOutBluffButton.interactable = show;
		declareBidSpotOnButton.interactable = show;

		if (show) {
			enterBidTextColor.a = full;
			callOutBluffTextColor.a = full;
			declareBidSpotOnTextColor.a = full;
		} else {
			enterBidTextColor.a = transparent;
			callOutBluffTextColor.a = transparent;
			declareBidSpotOnTextColor.a = transparent;
		}
	}

	void OnDestroy () {
		GamePlayer.ShowControlsEvent -= OnShowControls;
	}

}

