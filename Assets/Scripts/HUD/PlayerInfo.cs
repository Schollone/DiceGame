using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using MW_DiceGame;

public class PlayerInfo : MonoBehaviour {

	public Text playerNameText;
	public Outline playerNameOutline;
	public GameObject dieFaceContainer;
	public Image diceQuantityAsDieFace;
	public Slots slot;

	Colors color = Colors.Black;
	int dicesLeft = 5;

	void Awake () {
		GamePlayer.PlayerNameChangedEvent += OnPlayerNameChanged;
		GamePlayer.ColorChangedEvent += OnColorChanged;

		DiceCup.AvailableDicesChangedEvent += OnAvailableDicesChanged;

		HidePlayerInfoSlot ();
	}

	void Start () {
	}

	void OnPlayerNameChanged (Slots targetSlot, string newPlayerName) {
		if (slot == targetSlot) {
			playerNameText.text = newPlayerName;
			ShowPlayerInfoSlot ();
		}
	}

	void OnColorChanged (Slots targetSlot, Colors newColor) {
		if (slot == targetSlot) {
			this.color = newColor;
			playerNameText.color = color.GetColor ();
			if (newColor.Equals (Colors.Black)) {
				playerNameOutline.effectColor = new Color (1f, 1f, 1f, 0.5f);
			}

			Sprite dieFace = color.GetDieFaceImage (dicesLeft);
			diceQuantityAsDieFace.sprite = dieFace;
		}
	}

	void OnAvailableDicesChanged (Slots targetSlot, int dicesLeft) {
		if (slot == targetSlot) {
			this.dicesLeft = dicesLeft;

			Debug.Log ("Dices Left: " + dicesLeft);
			if (dicesLeft > 0) {
				Sprite dieFace = color.GetDieFaceImage (dicesLeft);
				diceQuantityAsDieFace.sprite = dieFace;
			} else {
				dieFaceContainer.SetActive (false);
			}

		}
	}



	void HidePlayerInfoSlot () {
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.SetActive (false);
		}
	}

	void ShowPlayerInfoSlot () {
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.SetActive (true);
		}
	}

	void OnDestroy () {
		GamePlayer.PlayerNameChangedEvent -= OnPlayerNameChanged;
		GamePlayer.ColorChangedEvent -= OnColorChanged;
		DiceCup.AvailableDicesChangedEvent -= OnAvailableDicesChanged;
	}

}