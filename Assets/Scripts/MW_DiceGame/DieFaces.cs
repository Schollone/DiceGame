using UnityEngine;
using System;
using System.Collections;

namespace MW_DiceGame {

	public enum DieFaces {
		Null,
		One,
		Two,
		Three,
		Four,
		Five,
		Six
	}

	static class DieFaceMethods {
		public static Sprite GetDieFaceImage (this DieFaces dieFace) {
			int diceValue = (int)dieFace;
			string path = "DieFaces/Dice_White_" + diceValue.ToString ();
			Sprite sprite = Resources.Load<Sprite> (path);
			return sprite;
		}

		public static int GetIndex (this DieFaces dieFace) {
			int diceValue = (int)dieFace;
			return diceValue;
		}

		public static int Length () {
			return Enum.GetValues (typeof(DieFaces)).Length;
		}

		public static DieFaces Parse (int index) {
			return (DieFaces)Enum.ToObject (typeof(DieFaces), index);
		}
	}

}