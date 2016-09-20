using UnityEngine;
using System;
using System.Collections;

namespace MW_DiceGame {

	public enum Slots {
		First,
		Second,
		Third,
		Fourth
	}

	public static class SlotMethods {
		public static int GetIndex (this Slots slot) {
			return (int)slot;
		}

		public static int Length () {
			return Enum.GetValues (typeof(Slots)).Length;
		}

		public static Slots Parse (int slotIndex) {
			return (Slots)Enum.ToObject (typeof(Slots), slotIndex);
		}

		public static Slots NextSlot (this Slots slot) {
			int index = slot.GetIndex ();
			index++;
			if (index >= Length ()) {
				index = 0;
			}
			return Parse (index);
		}
	}

}