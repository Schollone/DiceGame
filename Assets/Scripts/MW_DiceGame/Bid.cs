using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace MW_DiceGame {

	[System.Serializable]
	public struct Bid {

		public enum BidResult {
			Null,
			NoBluff,
			SpotOn,
			Bluff
		}

		public DieFaces dieFace;
		public int quantity;
		public NetworkInstanceId ownerId;

		public const int maxBidQuantity = 20;
		public const int minBidQuantity = 1;

		public const int maxBidDieFaceValue = 6;
		public const int minBidDieFaceValue = 1;


		public Bid (DieFaces dieFace, int quantity) {
			this.dieFace = dieFace;
			this.quantity = quantity;
			this.ownerId = NetworkInstanceId.Invalid;
		}

		public Bid (DieFaces dieFace, int quantity, NetworkInstanceId ownerId) : this (dieFace, quantity) {
			this.dieFace = dieFace;
			this.quantity = quantity;
			this.ownerId = ownerId;
		}

		public bool Exists () {
			return (this.dieFace != DieFaces.Null);
		}

		public override string ToString () {
			return string.Format ("Quantity: {0}, DieFace: {1}, OwnerId: {2}", quantity, dieFace, ownerId);
		}

		BidResult CheckIfWithin (Bid realBidOnTable) {
			Debug.Log ("this Quantity: " + this.quantity);
			if (this.quantity > maxBidQuantity || this.quantity < minBidQuantity) {
				Debug.LogError ("Bid is not in limits");
				return BidResult.Null;
			}

			if (realBidOnTable.Equals (null)) {
				Debug.LogError ("Parameter bid is null");
				return BidResult.Null;
			}
				
			if (this.dieFace.GetIndex () == realBidOnTable.dieFace.GetIndex ()) {
				if (this.quantity < realBidOnTable.quantity) {
					Debug.Log ("No Bluff");
					return BidResult.NoBluff;
				} else if (this.quantity == realBidOnTable.quantity) {
					Debug.Log ("SpotOn");
					return BidResult.SpotOn;
				} else {
					Debug.Log ("Bluff");
					return BidResult.Bluff;
				}
			} else {
				Debug.LogError ("Bids not comparable");
			}

			return BidResult.Null;
		}

		public bool CanBeReplacedWith (Bid newBid) {
			if (newBid.quantity > maxBidQuantity || newBid.quantity < minBidQuantity) {
				return false;
			}

			if (newBid.dieFace == DieFaces.Null) {
				return false;
			}

			if (newBid.quantity >= this.quantity) {
				if (newBid.dieFace.GetIndex () > this.dieFace.GetIndex ()) {
					return true;
				}
			}

			return false;
		}

		public bool IsBluff (Bid realBidOnTable) {
			BidResult bidResult = CheckIfWithin (realBidOnTable);

			switch (bidResult) {
				case BidResult.Bluff:
				case BidResult.SpotOn:
					{
						return true;
					}
				default:
					{
						return false;
					}
			}
		}

		public bool IsSpotOn (Bid realBidOnTable) {
			BidResult bidResult = CheckIfWithin (realBidOnTable);

			switch (bidResult) {
				case BidResult.SpotOn:
					{
						return true;
					}
				default:
					{
						return false;
					}
			}
		}

		/*public int IComparable<Bid>.CompareTo (Bid other) {
			if (other.Equals (null))
				return 1;

			if (this.quantity == other.quantity) {
				if (this.dieFace.GetIndex () == other.dieFace.GetIndex ()) {
					return 0;
				} else if (this.dieFace.GetIndex () < other.dieFace.GetIndex ()) {
					return -1;
				} else {
					return 1;
				}
			} else if (this.quantity < other.quantity) {
				return -1;
			} else {
				return 1;
			}
		}*/
	}

}