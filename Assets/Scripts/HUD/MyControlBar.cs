using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MW_DiceGame;

public class MyControlBar : MonoBehaviour {

	public GameObject bidController;
	public Button enterBidButton;
	public Button callOutBluffButton;
	public Button declareBidSpotOnButton;
	public Button lookUpDicesButton;
	public Button hideDicesButton;

	Text enterBidText;
	Text callOutBluffText;
	Text declareBidSpotOnText;
	Text lookUpDicesText;
	Text hideDicesText;

	void Awake () {
		GamePlayer.ShowControlsEvent += OnShowControls;
		GamePlayer.ActiveControlsEvent += OnActiveControls;

		enterBidText = enterBidButton.transform.GetChild (0).GetComponent<Text> ();
		callOutBluffText = callOutBluffButton.transform.GetChild (0).GetComponent<Text> ();
		declareBidSpotOnText = declareBidSpotOnButton.transform.GetChild (0).GetComponent<Text> ();
		lookUpDicesText = lookUpDicesButton.transform.GetChild (0).GetComponent<Text> ();
		hideDicesText = hideDicesButton.transform.GetChild (0).GetComponent<Text> ();
	}

	void OnShowControls (bool show) {
		Debug.Log ("OnShowControls: " + show);
		enterBidButton.interactable = show;
		callOutBluffButton.interactable = show;
		declareBidSpotOnButton.interactable = show;
		lookUpDicesButton.interactable = show;
		hideDicesButton.interactable = show;

		if (show) {
			enterBidText.color = ColorMethods.visibleDarkBrownColor;
			callOutBluffText.color = ColorMethods.visibleDarkBrownColor;
			declareBidSpotOnText.color = ColorMethods.visibleDarkBrownColor;
			lookUpDicesText.color = ColorMethods.visibleDarkBrownColor;
			hideDicesText.color = ColorMethods.visibleDarkBrownColor;
		} else {
			enterBidText.color = ColorMethods.transparentBlackColor;
			callOutBluffText.color = ColorMethods.transparentBlackColor;
			declareBidSpotOnText.color = ColorMethods.transparentBlackColor;
			lookUpDicesText.color = ColorMethods.transparentBlackColor;
			hideDicesText.color = ColorMethods.transparentBlackColor;
		}
	}

	void OnActiveControls (bool show) {
		Debug.Log ("OnActiveControls: " + show);
		enterBidButton.interactable = show;
		callOutBluffButton.interactable = show;
		declareBidSpotOnButton.interactable = show;

		if (show) {
			enterBidText.color = ColorMethods.visibleDarkBrownColor;
			callOutBluffText.color = ColorMethods.visibleDarkBrownColor;
			declareBidSpotOnText.color = ColorMethods.visibleDarkBrownColor;
		} else {
			enterBidText.color = ColorMethods.transparentBlackColor;
			callOutBluffText.color = ColorMethods.transparentBlackColor;
			declareBidSpotOnText.color = ColorMethods.transparentBlackColor;
		}
	}

	void OnDestroy () {
		GamePlayer.ShowControlsEvent -= OnShowControls;
	}

}

