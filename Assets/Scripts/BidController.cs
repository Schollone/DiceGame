using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MW_DiceGame;

public class BidController : MonoBehaviour {

	public delegate void BidAction (Bid bid);

	public static event BidAction OnEnterBid;


	public Text BidQuantityText;
	public Image BidDieFaceImage;


	const int maxBidQuantity = 20;
	const int minBidQuantity = 1;

	const int maxBidDieFaceValue = 6;
	const int minBidDieFaceValue = 1;

	int bidQuantity = 1;
	int bidDieFaceValue = 1;



	// Use this for initialization
	void Start () {
		UpdateBidQuantityDisplay ();
		UpdateBidDieFaceDisplay ();
	}

	public void EnterBid () {
		if (OnEnterBid != null) {
			Bid bid = new Bid ((DieFaces)Enum.ToObject (typeof(DieFaces), bidDieFaceValue), bidQuantity);
			OnEnterBid (bid);
		}

	}

	public void IncreaseBidQuantity () {
		if (bidQuantity + 1 > maxBidQuantity) {
			return;
		}
		bidQuantity++;
		UpdateBidQuantityDisplay ();
	}

	public void DecreaseBidQuantity () {
		if (bidQuantity - 1 < minBidQuantity) {
			return;
		}
		bidQuantity--;
		UpdateBidQuantityDisplay ();
	}

	public void IncreaseBidDieFace () {
		if (bidDieFaceValue + 1 > maxBidDieFaceValue) {
			return;
		}
		bidDieFaceValue++;
		UpdateBidDieFaceDisplay ();
	}

	public void DecreaseBidDieFace () {
		if (bidDieFaceValue - 1 < minBidDieFaceValue) {
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