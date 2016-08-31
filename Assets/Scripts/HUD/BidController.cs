using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using MW_DiceGame;

public class BidController : MonoBehaviour {

	public delegate void BidAction (Bid bid);

	//public static event BidAction OnEnterBid;


	public Text BidQuantityText;
	public Image BidDieFaceImage;

	public Button increaseQuantityButton;
	public Button decreaseQuantityButton;
	public Button increaseDieFaceButton;
	public Button decreaseDieFaceButton;

	int bidQuantity = 1;
	int bidDieFaceValue = 1;

	void Awake () {
		GamePlayer.ShowControlsEvent += OnShowControls;
	}

	void Start () {
		Debug.Log ("BidController - Start");
		NetworkClient client = Lobby.singleton.client;
		client.RegisterHandler (ActionMsg.EnterBid, OnBidEntered);

		bidQuantity = 1;
		bidDieFaceValue = 1;

		DisplayBidQuantity ();
		DisplayBidDieFace ();
		UpdateInteractableButtons ();
	}

	void OnShowControls (bool show) {
		Debug.Log ("BidController: OnShowControls - " + show);

		if (show) {
			UpdateInteractableButtons ();
		} else {
			increaseQuantityButton.interactable = false;
			decreaseQuantityButton.interactable = false;
			increaseDieFaceButton.interactable = false;
			decreaseDieFaceButton.interactable = false;
		}
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
		NetworkClient client = Lobby.singleton.client;
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

			UpdateDecreaseDieFaceBtn (currentBid.dieFace.GetIndex ());
			UpdateIncreaseDieFaceBtn ();
			UpdateIncreaseQuantityBtn ();

		} else if (bidQuantity > currentBid.quantity) {
			this.decreaseQuantityButton.interactable = true;

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
		} else {
			this.decreaseQuantityButton.interactable = true;
		}
	}

	void UpdateDecreaseDieFaceBtn (int lowestComparativeValue) {
		if (bidDieFaceValue == lowestComparativeValue) {
			this.decreaseDieFaceButton.interactable = false;
		} else {
			this.decreaseDieFaceButton.interactable = true;
		}
	}

	void UpdateIncreaseDieFaceBtn () {
		if (bidDieFaceValue == Bid.maxBidDieFaceValue) {
			this.increaseDieFaceButton.interactable = false;
		} else {
			this.increaseDieFaceButton.interactable = true;
		}
	}

	void UpdateIncreaseQuantityBtn () {
		if (bidQuantity == Bid.maxBidQuantity) {
			this.increaseQuantityButton.interactable = false;
		} else {
			this.increaseQuantityButton.interactable = true;
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
		Sprite sprite = Colors.White.GetDieFaceImage (bidDieFaceValue);
		BidDieFaceImage.sprite = sprite;
	}
}