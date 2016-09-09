﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using MW_DiceGame;
using Prototype.NetworkLobby;

public class BidController : MonoBehaviour {

	public Text BidQuantityText;
	public Image BidDieFaceImage;

	public Button enterBidButton;
	public Button increaseQuantityButton;
	public Button decreaseQuantityButton;
	public Button increaseDieFaceButton;
	public Button decreaseDieFaceButton;

	int bidQuantity = 1;
	int bidDieFaceValue = 1;

	bool isMyTurn;

	void Awake () {
		GamePlayer.EventUnlockControls += OnUnlockControls;
		GamePlayer.EventLockControls += OnLockControls;
		GamePlayer.EventOnBidChanged += OnBidChanged;
		GamePlayer.ItIsMyTurnEvent += OnIsMyTurn;

		isMyTurn = false;
	}

	void Start () {
		//Debug.Log ("BidController - Start");
		NetworkClient client = LobbyManager.singleton.client;
		//client.RegisterHandler (ActionMsg.EnterBid, OnBidEntered);

		ResetBidDisplay ();
		LockAllBidButtons ();
	}





	void OnUnlockControls (bool isMyTurn, bool bidAlreadyExists) {
		Debug.Log ("BidController: OnUnlockControls " + this.isMyTurn);
		ResetBidDisplay ();

		UpdateBidButtons ();
	}

	void OnLockControls (bool isMyTurn, bool bidAlreadyExists) {
		Debug.Log ("BidController: OnLockControls " + this.isMyTurn);
		LockAllBidButtons ();
	}

	void OnBidChanged (bool isMyTurn, bool bidAlreadyExists) {
		Debug.Log ("BidController: OnBidChanged " + Table.singleton.currentBid);
		Bid bid = Table.singleton.currentBid;

		if (bid.Exists ()) {
			this.bidQuantity = bid.quantity;
			this.bidDieFaceValue = bid.dieFace.GetIndex ();
			DisplayBidQuantity ();
			DisplayBidDieFace ();
		} else {
			ResetBidDisplay ();
		}

		UpdateBidButtons ();
	}

	void OnIsMyTurn (bool isMyTurn, bool bidAleadyExists) {
		Debug.Log ("BidController: OnIsMyTurn " + isMyTurn);

		this.isMyTurn = isMyTurn;

		UpdateBidButtons ();
	}



	public void IncreaseBidQuantity () {
		if (bidQuantity + 1 > Bid.maxBidQuantity) {
			return;
		}
		bidQuantity++;

		DisplayBidQuantity ();
		DetermineBidChangerButtons ();
		DetermineBidEnterButton ();
	}

	public void DecreaseBidQuantity () {
		if (bidQuantity - 1 < Bid.minBidQuantity) {
			return;
		}
		bidQuantity--;

		CheckDieFaceDisplay ();
		DisplayBidQuantity ();
		DetermineBidChangerButtons ();
		DetermineBidEnterButton ();
	}

	public void IncreaseBidDieFace () {
		if (bidDieFaceValue + 1 > Bid.maxBidDieFaceValue) {
			return;
		}
		bidDieFaceValue++;

		DisplayBidDieFace ();
		DetermineBidChangerButtons ();
		DetermineBidEnterButton ();
	}

	public void DecreaseBidDieFace () {
		if (bidDieFaceValue - 1 < Bid.minBidDieFaceValue) {
			return;
		}
		bidDieFaceValue--;

		DisplayBidDieFace ();
		DetermineBidChangerButtons ();
		DetermineBidEnterButton ();
	}

	public void EnterBid () {
		Debug.Log ("EnterBid");
		SendEnterBidAction ();
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
		Debug.Log ("Send to Server new Enter: " + msg.bid);
		client.Send (ActionMsg.EnterBid, msg);
	}

	/*void OnBidEntered (NetworkMessage netMsg) {
		//Debug.Log ("BitController ___ OnBidEntered");
		var msg = netMsg.ReadMessage<ActionMessage> ();
		this.bidQuantity = msg.bid.quantity;
		this.bidDieFaceValue = msg.bid.dieFace.GetIndex ();

		UpdateButtons ();

		DisplayBidQuantity ();
		DisplayBidDieFace ();
	}*/








	void ResetBidDisplay () {
		Debug.Log ("ResetBidDisplay");
		bidQuantity = Bid.minBidQuantity;
		bidDieFaceValue = Bid.minBidDieFaceValue;

		DisplayBidQuantity ();
		DisplayBidDieFace ();

		//UpdateInteractiveButtons ();
	}

	void UpdateBidButtons () {
		if (isMyTurn) {
			DetermineBidEnterButton ();
			DetermineBidChangerButtons ();
		} else {
			LockAllBidButtons ();
		}
	}

	void LockAllBidButtons () {
		increaseQuantityButton.interactable = false;
		decreaseQuantityButton.interactable = false;
		increaseDieFaceButton.interactable = false;
		decreaseDieFaceButton.interactable = false;
		enterBidButton.interactable = false;
	}











	void DetermineBidChangerButtons () {
		Bid currentBid = Table.singleton.currentBid;
		//Debug.LogWarning (currentBid);

		if (currentBid.Exists ()) {
			DetermineBidChangerButtonsWithBid (currentBid);
		} else {
			DetermineBidChangerButtonsWithoutBid ();
		}
			
	}

	void DetermineBidChangerButtonsWithBid (Bid currentBid) {
		if (bidQuantity == currentBid.quantity) {
			this.decreaseQuantityButton.interactable = false;
			//Color color = new Color ();
			//this.decreaseQuantityText.color = ColorMethods.transparentBlackColor;

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

	void DetermineBidChangerButtonsWithoutBid () {
		UpdateDecreaseQuantityBtn ();
		UpdateDecreaseDieFaceBtn (Bid.minBidDieFaceValue);
		UpdateIncreaseDieFaceBtn ();
		UpdateIncreaseQuantityBtn ();
	}



	void UpdateDecreaseQuantityBtn () {
		//Debug.Log ("bidQuantity == Bid.minBidQuantity: " + bidQuantity + " == " + Bid.minBidQuantity);
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

	void DetermineBidEnterButton () {
		Bid currentBid = Table.singleton.currentBid;

		if (currentBid.Exists ()) {
			if (bidQuantity == currentBid.quantity && bidDieFaceValue == currentBid.dieFace.GetIndex ()) {
				enterBidButton.interactable = false;
				return;
			}
		}

		enterBidButton.interactable = true;

	}


	void DisplayBidQuantity () {
		BidQuantityText.text = bidQuantity.ToString ();
	}

	void DisplayBidDieFace () {
		Sprite sprite = Colors.Empty.GetDieFaceImage (bidDieFaceValue);
		BidDieFaceImage.sprite = sprite;
	}


	void OnDestroy () {
		GamePlayer.EventUnlockControls -= OnUnlockControls;
		GamePlayer.EventLockControls -= OnLockControls;
		GamePlayer.EventOnBidChanged -= OnBidChanged;
		GamePlayer.ItIsMyTurnEvent -= OnIsMyTurn;
	}
}