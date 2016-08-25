using UnityEngine;
using System;
using System.Collections;

namespace MW_DiceGame {

	public enum Colors {
		Black,
		Blue,
		Green,
		Red,
		White,
		Yellow
	}

	static class ColorMethods {
		public static UnityEngine.Color GetColor (this Colors color) {
			switch (color) {
				case Colors.Black:
					return UnityEngine.Color.black;
				case Colors.Blue:
					return UnityEngine.Color.blue;
				case Colors.Green:
					return new Color (0f, 0.68f, 0.11f);
				case Colors.Red:
					return new Color (0.68f, 0f, 0f);
				case Colors.White:
					return UnityEngine.Color.white;
				case Colors.Yellow:
					return UnityEngine.Color.yellow;
				default:
					return UnityEngine.Color.magenta;
			}
		}

		public static Material GetDiceMaterial (this Colors color) {
			return Resources.Load<Material> ("Materials/Dice_6_Dots_" + color.ToString ());
		}

		public static Sprite GetDieFaceImage (this Colors color, int number = 5) {
			string path = "DieFaces/Dice_" + color.ToString () + "_" + number.ToString ();
			Sprite sprite = Resources.Load<Sprite> (path);
			return sprite;
		}

		public static Sprite GetDieFaceImage2 (Colors color, int number = 5) {
			string path = "DieFaces/Dice_" + color.ToString () + "_" + number.ToString ();
			Sprite sprite = Resources.Load<Sprite> (path);
			return sprite;
		}

		public static int GetIndex (this Colors color) {
			return (int)color;
		}

		public static int Length () {
			return Enum.GetValues (typeof(Colors)).Length;
		}

		public static Colors Parse (int colorIndex) {
			return (Colors)Enum.ToObject (typeof(Colors), colorIndex);
		}

		public static Colors NextColor (this Colors color) {
			int index = color.GetIndex ();
			index++;
			if (index >= Length ()) {
				index = 0;
			}
			return Parse (index);
		}
	}

}