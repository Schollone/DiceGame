using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using MW_DiceGame;
using Prototype.NetworkLobby;

public class BidController : MonoBehaviour {

	public delegate void BidAction (Bid bid);

	//public static event BidAction OnEnterBid;


	public Text BidQuantityText;
	public Image BidDieFaceImage;

	public Button increaseQuantityButton;
	public Text increaseQuantityText;
	public Button decreaseQuantityButton;
	public Text decreaseQuantityText;
	public Button increaseDieFaceButton;
	public Text increaseDieFaceText;
	public Button decreaseDieFaceButton;
	public Text decreaseDieFaceText;

	int bidQuantity = 1;
	int bidDieFaceValue = 1;

	//static Color visibleColor = new Color (131.0f / 255.0f, 67.0f / 255.0f, 32.0f / 255.0f, 1f);
	//static Color transparentColor = new Color (0f, 0f, 0f, 0.5f);

	void Awake () {
		Table.UnlockControlsEvent += OnUnlockControls;
		Table.LockControlsEvent += OnLockControls;
		Table.BidDoesNotExistEvent += OnBidDoesNotExist;
		GamePlayer.ItIsMyTurnEvent += OnIsMyTurn;
		//GamePlayer.HideControlsEvent += OnHideControls;
		//increaseQuantityTextColor = increaseQuantityButton.transform.GetChild (0).GetComponent<Text> ().color;
		//decreaseQuantityTextColor = decreaseQuantityButton.transform.GetChild (0).GetComponent<Text> ().color;
		//increaseDieFaceTextColor = increaseDieFaceButton.transform.GetChild (0).GetComponent<Text> ().color;
		//decreaseDieFaceTextColor = decreaseDieFaceButton.transform.GetChild (0).GetComponent<Text> ().color;
	}

	void Start () {
		Debug.Log ("BidController - Start");
		NetworkClient client = LobbyManager.singleton.client;
		client.RegisterHandler (ActionMsg.EnterBid, OnBidEntered);

		bidQuantity = 1;
		bidDieFaceValue = 1;

		DisplayBidQuantity ();
		DisplayBidDieFace ();
		UpdateInteractableButtons ();
	}



	void OnUnlockControls () {
		//UpdateInteractableButtons ();
	}

	void OnLockControls () {
		LockBidButtons ();
	}

	void OnBidDoesNotExist (bool isMyTurn) {
		UpdateInteractableButtons ();
		bidQuantity = 1;
		bidDieFaceValue = 1;
		DisplayBidQuantity ();
		DisplayBidDieFace ();
	}

	void OnIsMyTurn (bool isMyTurn) {
		Debug.Log ("BidController: OnIsMyTurn " + isMyTurn);

		if (isMyTurn) {
			UpdateInteractableButtons ();
		} else {
			LockBidButtons ();
		}

	}

	void LockBidButtons () {
		increaseQuantityButton.interactable = false;
		decreaseQuantityButton.interactable = false;
		increaseDieFaceButton.interactable = false;
		decreaseDieFaceButton.interactable = false;
	}




	public void IncreaseBidQuantity () {
		if (bidQuantity + 1 > Bid.maxBidQuantity) {
			return;
		}
		bidQuantity++;
		DisplayBidQuantity ();
		UpdateInteractableButtons ();
	}

	public void DecreaseBidQuantity () {
		if (bidQuantity - 1 < Bid.minBidQuantity) {
			return;
		}
		bidQuantity--;
		CheckDieFaceDisplay ();
		DisplayBidQuantity ();
		UpdateInteractableButtons ();
	}

	public void IncreaseBidDieFace () {
		if (bidDieFaceValue + 1 > Bid.maxBidDieFaceValue) {
			return;
		}
		bidDieFaceValue++;
		DisplayBidDieFace ();
		UpdateInteractableButtons ();
	}

	public void DecreaseBidDieFace () {
		if (bidDieFaceValue - 1 < Bid.minBidDieFaceValue) {
			return;
		}
		bidDieFaceValue--;
		DisplayBidDieFace ();
		UpdateInteractableButtons ();
	}

	public void EnterBid () {
		Debug.Log ("EnterBid");
		SendEnterBidAction ();

		/*if (OnEnterBid != null) {
			OnEnterBid (bid);
		}*/
	}






	void SendEnterBidAction () {
		NetworkClient client = LobbyManager.singleton.client;
		if (client == null || !client.isConnected) {
			return;
		}

		var msg = new ActionMessage ();
		Bid bid = new Bid ((DieFaces)Enum.ToObject (typeof(DieFaces), bidDieFaceValue), bidQuantity);
		msg.bid = bid;
		msg.connectionId = client.connection.connectionId;
		client.Send (ActionMsg.EnterBid, msg);
	}

	void OnBidEntered (NetworkMessage netMsg) {
		Debug.Log ("BitController ___ OnBidEntered");
		var msg = netMsg.ReadMessage<ActionMessage> ();
		this.bidQuantity = msg.bid.quantity;
		this.bidDieFaceValue = msg.bid.dieFace.GetIndex ();

		UpdateInteractableButtons ();


		DisplayBidQuantity ();
		DisplayBidDieFace ();
	}

	void UpdateInteractableButtons () {
		Bid currentBid = Table.singleton.currentBid;
		Debug.LogWarning (currentBid);

		if (currentBid.Exists ()) {
			UpdateButtonsIfCurrentBidExists (currentBid);
		} else {
			UpdateButtonsIfCurrentBidExistsNot ();
		}
			
	}

	void UpdateButtonsIfCurrentBidExists (Bid currentBid) {
		if (bidQuantity == currentBid.quantity) {
			this.decreaseQuantityButton.interactable = false;
			Color color = new Color ();
			this.decreaseQuantityText.color = ColorMethods.transparentBlackColor;

			UpdateDecreaseDieFaceBtn (currentBid.dieFace.GetIndex ());
			UpdateIncreaseDieFaceBtn ();
			UpdateIncreaseQuantityBtn ();

		} else if (bidQuantity > currentBid.quantity) {
			this.decreaseQuantityButton.interactable = true;
			this.decreaseQuantityText.color = ColorMethods.visibleDarkBrownColor;

			UpdateDecreaseDieFaceBtn (Bid.minBidDieFaceValue);
			UpdateIncreaseDieFaceBtn ();
			UpdateIncreaseQuantityBtn ();
		}
	}

	void UpdateButtonsIfCurrentBidExistsNot () {
		UpdateDecreaseQuantityBtn ();
		UpdateDecreaseDieFaceBtn (Bid.minBidDieFaceValue);
		UpdateIncreaseDieFaceBtn ();
		UpdateIncreaseQuantityBtn ();
	}



	void UpdateDecreaseQuantityBtn () {
		Debug.Log ("bidQuantity == Bid.minBidQuantity: " + bidQuantity + " == " + Bid.minBidQuantity);
		if (bidQuantity == Bid.minBidQuantity) {
			this.decreaseQuantityButton.interactable = false;
			this.decreaseQuantityText.color = ColorMethods.transparentBlackColor;
		} else {
			this.decreaseQuantityButton.interactable = true;
			this.decreaseQuantityText.color = ColorMethods.visibleDarkBrownColor;
		}
	}

	void UpdateDecreaseDieFaceBtn (int lowestComparativeValue) {
		if (bidDieFaceValue == lowestComparativeValue) {
			this.decreaseDieFaceButton.interactable = false;
			this.decreaseDieFaceText.color = ColorMethods.transparentBlackColor;
		} else {
			this.decreaseDieFaceButton.interactable = true;
			this.decreaseDieFaceText.color = ColorMethods.visibleDarkBrownColor;
		}
	}

	void UpdateIncreaseDieFaceBtn () {
		if (bidDieFaceValue == Bid.maxBidDieFaceValue) {
			this.increaseDieFaceButton.interactable = false;
			this.increaseDieFaceText.color = ColorMethods.transparentBlackColor;
		} else {
			this.increaseDieFaceButton.interactable = true;
			this.increaseDieFaceText.color = ColorMethods.visibleDarkBrownColor;
		}
	}

	void UpdateIncreaseQuantityBtn () {
		if (bidQuantity == Bid.maxBidQuantity) {
			this.increaseQuantityButton.interactable = false;
			this.increaseQuantityText.color = ColorMethods.transparentBlackColor;
		} else {
			this.increaseQuantityButton.interactable = true;
			this.increaseQuantityText.color = ColorMethods.visibleDarkBrownColor;
		}
	}

	void CheckDieFaceDisplay () {
		Bid currentBid = Table.singleton.currentBid;
		if (bidQuantity == currentBid.quantity) {
			if (bidDieFaceValue < currentBid.dieFace.GetIndex ()) {
				bidDieFaceValue = currentBid.dieFace.GetIndex ();
				DisplayBidDieFace ();
			}
		}
	}


	void DisplayBidQuantity () {
		BidQuantityText.text = bidQuantity.ToString ();
	}

	void DisplayBidDieFace () {
		Sprite sprite = Colors.Empty.GetDieFaceImage (bidDieFaceValue);
		BidDieFaceImage.sprite = sprite;
	}


	void OnDestroy () {
		Table.UnlockControlsEvent -= OnUnlockControls;
		Table.LockControlsEvent -= OnLockControls;
		Table.BidDoesNotExistEvent -= OnBidDoesNotExist;
		GamePlayer.ItIsMyTurnEvent -= OnIsMyTurn;
	}
}