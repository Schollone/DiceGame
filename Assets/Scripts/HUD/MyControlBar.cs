using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MW_DiceGame;

public class MyControlBar : MonoBehaviour {

	public Button callOutBluffButton;
	public Button declareBidSpotOnButton;
	public Button lookUpDicesButton;
	public Button hideDicesButton;

	bool isMyTurn;

	void Awake () {
		GamePlayer.EventUnlockControls += OnUnlockControls;
		GamePlayer.EventLockControls += OnLockControls;
		GamePlayer.EventOnBidChanged += OnBidChanged;
		GamePlayer.ItIsMyTurnEvent += OnItIsMyTurn;

		isMyTurn = false;
	}

	void OnUnlockControls (bool isMyTurn, bool bidAlreadyExists) {
		Debug.Log ("Aktiviere Look und Hide Buttons");
		lookUpDicesButton.interactable = true;
		hideDicesButton.interactable = true;
		lookUpDicesButton.gameObject.SetActive (true);
	}

	void OnLockControls (bool isMyTurn, bool bidAlreadyExists) {
		Debug.Log ("Deaktiviere alle Buttons");
		//enterBidButton.interactable = false;
		callOutBluffButton.interactable = false;
		declareBidSpotOnButton.interactable = false;
		lookUpDicesButton.interactable = false;
		hideDicesButton.interactable = false;
	}

	void OnBidChanged (bool isMyTurn, bool bidAlreadyExists) {
		Debug.Log ("Deaktiviere Action Buttons - ItIsMyTurn: " + isMyTurn);

		UpdateActionButtons (bidAlreadyExists);
	}

	void OnItIsMyTurn (bool isMyTurn, bool bidAlreadyExists) {
		Debug.LogWarning ("Aktualisiere Action Buttons - ItIsMyTurn: " + isMyTurn);

		this.isMyTurn = isMyTurn;

		UpdateActionButtons (bidAlreadyExists);
	}

	void UpdateActionButtons (bool bidAlreadyExists) {
		if (bidAlreadyExists) {
			//enterBidButton.interactable = false;
			callOutBluffButton.interactable = isMyTurn;
			declareBidSpotOnButton.interactable = isMyTurn;
		} else {
			//enterBidButton.interactable = isMyTurn;
			callOutBluffButton.interactable = false;
			declareBidSpotOnButton.interactable = false;
		}
	}

	void OnDestroy () {
		GamePlayer.EventUnlockControls -= OnUnlockControls;
		GamePlayer.EventLockControls -= OnLockControls;
		GamePlayer.EventOnBidChanged -= OnBidChanged;
		GamePlayer.ItIsMyTurnEvent -= OnItIsMyTurn;
	}

}

