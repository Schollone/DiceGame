using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MW_DiceGame;

public class EventDisplayer : MonoBehaviour {

	public Text eventText;
	public Outline outline;

	void Awake () {
		Table.UpdateEventDisplayEvent += OnUpdateDisplay;
		Table.EventDisplayVisibilityEvent += OnDisplayVisibility;
		eventText.enabled = false;
	}

	void OnUpdateDisplay (string playername, Colors color, string actionDescription) {
		eventText.color = color.GetColor ();

		outline.effectColor = Colors.Black.GetColor ();
		if (color.Equals (Colors.Black)) {
			outline.effectColor = new Color (1f, 1f, 1f, 0.5f);
		}

		eventText.text = playername + " " + actionDescription;

		eventText.enabled = true;
	}

	void OnDisplayVisibility (bool visibility) {
		eventText.enabled = visibility;
	}

	void EnableText () {
		eventText.enabled = true;
	}

	void DisableText () {
		eventText.enabled = false;
	}
}
