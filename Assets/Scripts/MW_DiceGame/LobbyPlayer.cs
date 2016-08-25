using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using System.Collections;

namespace MW_DiceGame {

	public class LobbyPlayer : NetworkLobbyPlayer {

		[SyncVar]
		public string playerName;
		[SyncVar (hook = "OnColorChanged")]
		public Colors color;
		[SyncVar]
		public Slots slotId;

		private string tmpPlayerName;

		public override void OnClientEnterLobby () {
			base.OnClientEnterLobby ();

			tmpPlayerName = playerName;
			LoadDieFace (color);
			slotId = SlotMethods.Parse (slot);
		}

		GUIContent content = new GUIContent ("");




		void OnColorChanged (Colors newColor) {
			Debug.LogFormat ("newColor={0}, color={1}", newColor, color);
			this.color = newColor;
			LoadDieFace (newColor);
		}

		void LoadDieFace (Colors color) {
			Sprite sprite = color.GetDieFaceImage (slotId.GetIndex () + 1);
			content = new GUIContent (sprite.texture);
		}




		[Command]
		void CmdSaveSettings (string newName, Slots slotId) {
			Debug.Log ("CmdSaveSettings");
			this.slotId = slotId;
			this.playerName = newName;
		}

		[Command]
		void CmdUpdateColorImage () {
			this.color = this.color.NextColor ();
			LoadDieFace (this.color);
		}




		void OnGUI () {

			if (!ShowLobbyGUI)
				return;

			var lobby = Lobby.singleton as Lobby;
			if (lobby) {
				if (!lobby.showLobbyGUI)
					return;

				string loadedSceneName = SceneManager.GetSceneAt (0).name;
				if (loadedSceneName != lobby.lobbyScene)
					return;
			}

			Rect rec = new Rect (100 + slotId.GetIndex () * 100, 10, 100, 230);
			string readyOrNot = readyToBegin ? "Ready" : "Not Ready";

			if (isLocalPlayer) {

				GUI.Box (rec, "Player " + (slotId.GetIndex () + 1));

				rec.x += 10;
				rec.y += 30;
				if (GUI.Button (new Rect (rec.x + 20, rec.y, 40, 40), content)) {
					CmdUpdateColorImage ();
				}

				rec.y += 45;
				rec.width = 80;
				rec.height = 20;
				tmpPlayerName = GUI.TextField (rec, tmpPlayerName);

				rec.y += 30;
				GUI.Label (rec, readyOrNot);

				rec.y += 30;
				if (GUI.Button (rec, "Save")) {
					//playerName = tmpPlayerName;
					Debug.LogFormat ("SaveSettings {0}, {1}, {2}", tmpPlayerName, color, slotId);
					CmdSaveSettings (tmpPlayerName, slotId);
				}

				rec.y += 30;
				if (readyToBegin) {
					if (GUI.Button (rec, "Unready")) {
						SendNotReadyToBeginMessage ();
					}
				} else {
					if (GUI.Button (rec, "Ready")) {
						SendReadyToBeginMessage ();
					}
					rec.y += 25;
					if (GUI.Button (rec, "Remove")) {
						ClientScene.RemovePlayer (GetComponent<NetworkIdentity> ().playerControllerId);
					}
				}

			} else {

				GUI.Box (rec, "Player " + (slotId.GetIndex () + 1));

				rec.x += 10;
				rec.y += 30;
				GUI.Button (new Rect (rec.x + 20, rec.y, 40, 40), content);

				rec.y += 45;
				rec.width = 80;
				rec.height = 20;
				GUI.Label (rec, playerName);

				rec.y += 30;
				GUI.Label (rec, readyOrNot);
			}
			
		}
	}

}