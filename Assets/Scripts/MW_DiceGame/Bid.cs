using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace MW_DiceGame {

	[System.Serializable]
	public struct Bid : IComparable<Bid> {

		public DieFaces dieFace;
		public int quantity;
		public NetworkInstanceId ownerId;

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

		public override string ToString () {
			return string.Format ("Quantity: {0}, DieFace: {1}, OwnerId: {2}", quantity, dieFace, ownerId);
		}

		#region IComparable implementation

		int IComparable<Bid>.CompareTo (Bid other) {
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
		}

		#endregion
	}

}