using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MW_DiceGame;

public class Controlbar : MonoBehaviour {

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
		lookUpDicesButton.interactable = true;
		hideDicesButton.interactable = true;
		lookUpDicesButton.gameObject.SetActive (true);
	}

	void OnLockControls (bool isMyTurn, bool bidAlreadyExists) {
		callOutBluffButton.interactable = false;
		declareBidSpotOnButton.interactable = false;
		lookUpDicesButton.interactable = false;
		hideDicesButton.interactable = false;
	}

	void OnBidChanged (bool isMyTurn, bool bidAlreadyExists) {
		UpdateActionButtons (bidAlreadyExists);
	}

	void OnItIsMyTurn (bool isMyTurn, bool bidAlreadyExists) {
		this.isMyTurn = isMyTurn;

		UpdateActionButtons (bidAlreadyExists);
	}

	void UpdateActionButtons (bool bidAlreadyExists) {
		if (bidAlreadyExists) {
			callOutBluffButton.interactable = isMyTurn;
			declareBidSpotOnButton.interactable = isMyTurn;
		} else {
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