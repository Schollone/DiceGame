using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace MW_DiceGame {

	public class Dice : NetworkBehaviour {

		[SyncVar]
		public NetworkInstanceId diceCupId;

		[SyncVar (hook = "OnDieFaceChange")]
		public DieFaces dieFace;

		[SyncVar (hook = "OnColorChange")]
		public Colors color;

		private Colors col;
		private Transform diceTransform;
		private Rigidbody rigid;
		private bool diceThrown = false;


		void Awake () {
			diceTransform = transform;
			rigid = GetComponent<Rigidbody> ();
			diceThrown = false;
		}

		public override void OnStartClient () {
			base.OnStartClient ();

			GameObject go = ClientScene.FindLocalObject (diceCupId);
			transform.SetParent (go.GetComponent<SpawnManager> ().container.transform);
			OnColorChange (color);
		}

		public override void OnStartLocalPlayer () {
			base.OnStartLocalPlayer ();
		}





		void OnDieFaceChange (DieFaces dieFace) {
			this.dieFace = dieFace;

			Debug.LogWarning ("Dice - OnDieFaceChange: " + dieFace);

			diceThrown = false;

			GameObject diceCup = ClientScene.FindLocalObject (diceCupId);
			diceCup.GetComponent<DiceCup> ().UpdateDieFaceValueDisplay ();
		}

		void OnColorChange (Colors color) {
			this.color = color;

			GetComponent<MeshRenderer> ().material = color.GetDiceMaterial ();
		}

		public void ResetDice () {
			this.diceThrown = false;
		}





		void OnCollisionStay (Collision other) {
			if (other.collider.CompareTag ("Tabletop")) {
				if (diceThrown) {
					return;
				}

				int velocity = Mathf.RoundToInt (rigid.velocity.magnitude * 100);
				//Debug.Log(velocity + " - " + rigid.velocity.magnitude);
				if (velocity == 0) {
					dieFace = CalculateDieFace ();
					diceThrown = true;
				}
			}
		}

		DieFaces CalculateDieFace () {
			if (!gameObject.activeInHierarchy) {
				return DieFaces.Null;
			}

			int result = 0;

			int xOrientation = (int)diceTransform.rotation.eulerAngles.x;
			int zOrientation = (int)diceTransform.rotation.eulerAngles.z;

			xOrientation = IdealizeAngle (xOrientation);
			zOrientation = IdealizeAngle (zOrientation);

			if (xOrientation == 0) {
				if (zOrientation == 0) {
					result = 5;
				} else if (zOrientation == 90) {
					result = 4;
				} else if (zOrientation == 180) {
					result = 2;
				} else if (zOrientation == 270) {
					result = 3;
				}
			} else if (IsSix (xOrientation)) {
				result = 6;
			} else if (IsOne (xOrientation)) {
				result = 1;
			}

			return DieFaceMethods.Parse (result);
		}

		bool IsOne (int value) {
			bool result = false;
			if (value == 270) {
				result = true;
			}
			return result;
		}

		bool IsSix (int value) {
			bool result = false;
			if (value == 90) {
				result = true;
			}
			return result;
		}

		int IdealizeAngle (float angle) {
			for (int i = 0; i < 4; i++) {
				int targetAngle = i * 90;
				int delta = (int)Mathf.DeltaAngle (angle, targetAngle);
				delta = Math.Abs (delta);
				if (Math.Abs (delta) <= 40) {
					return i * 90;
				}
			}

			return 0;
		}

	}

}