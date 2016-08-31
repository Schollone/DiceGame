using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System;
using System.Collections;
using MW_DiceGame;

public class BidController : MonoBehaviour {

	public delegate void BidAction (Bid bid);

	public static event BidAction OnEnterBid;


	public Text BidQuantityText;
	public Image BidDieFaceImage;


	int minQuantity = Bid.minBidQuantity;
	int minDieFaceValue = Bid.minBidDieFaceValue;

	int bidQuantity = 1;
	int bidDieFaceValue = 1;

	void Start () {
		UpdateBidQuantityDisplay ();
		UpdateBidDieFaceDisplay ();

		Debug.Log ("BidController - Start");
		NetworkClient client = Lobby.singleton.client;
		client.RegisterHandler (ActionMsg.EnterBid, OnBidEntered);
	}

	void OnBidEntered (NetworkMessage netMsg) {
		Debug.Log ("BitController ___ OnBidEntered");
		var msg = netMsg.ReadMessage<ActionMessage> ();
		this.bidQuantity = msg.bid.quantity;
		this.bidDieFaceValue = msg.bid.dieFace.GetIndex ();

		UpdateBidQuantityDisplay ();
		UpdateBidDieFaceDisplay ();
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







	public void IncreaseBidQuantity () {
		if (bidQuantity + 1 > Bid.maxBidQuantity) {
			return;
		}
		bidQuantity++;
		UpdateBidQuantityDisplay ();
	}

	public void DecreaseBidQuantity () {
		if (bidQuantity - 1 < minQuantity) {
			return;
		}
		bidQuantity--;
		UpdateBidQuantityDisplay ();
	}

	public void IncreaseBidDieFace () {
		if (bidDieFaceValue + 1 > Bid.maxBidDieFaceValue) {
			return;
		}
		bidDieFaceValue++;
		UpdateBidDieFaceDisplay ();
	}

	public void DecreaseBidDieFace () {
		if (bidDieFaceValue - 1 < minDieFaceValue) {
			return;
		}
		bidDieFaceValue--;
		UpdateBidDieFaceDisplay ();
	}






	void UpdateBidQuantityDisplay () {
		BidQuantityText.text = bidQuantity.ToString ();
	}

	void UpdateBidDieFaceDisplay () {
		Sprite sprite = Colors.White.GetDieFaceImage (bidDieFaceValue);
		BidDieFaceImage.sprite = sprite;
	}
}