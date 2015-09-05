using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Common;
using Extension;

namespace Tutorial {
	public class TutorialUnit : CommonUnit {
		public override void OnStartLocalPlayer() {
			base.OnStartLocalPlayer();
			TutorialUnitManager.Instance.InitializeObjectList();
			TutorialUnitManager.Instance.getAllObjects().Add(this.gameObject);
		}

		protected void OnGUI() {
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.black;
			style.alignment = TextAnchor.MiddleCenter;
			Vector3 healthPosition = Camera.main.WorldToScreenPoint(this.transform.position);
			Rect rectPosition = new Rect(healthPosition.x - 50f, (Screen.height - healthPosition.y) - 45f, 100f, 25f);
			GUI.Label(rectPosition, new GUIContent(this.currentHealth.ToString() + "/" + this.maxHealth.ToString()), style);
		}

		public override void SetSelect() {
			if (!this.canBeSelected) {
				return;
			}
			this.isSelected = true;
			TutorialRing ring = this.GetComponentInChildren<TutorialRing>();
			if (ring != null) {
				ring.isSelected = true;
				if (this.isEnemy) {
					ring.SetColor(Color.red);
				}
				else {
					ring.SetColor(Color.green);
				}
			}
		}

		public override void SetDeselect() {
			if (!this.canBeSelected) {
				return;
			}
			this.isSelected = false;
			TutorialRing ring = this.GetComponentInChildren<TutorialRing>();
			if (ring != null) {
				ring.isSelected = false;
				if (this.isEnemy) {
					ring.SetColor(Color.red);
				}
				else {
					ring.SetColor(Color.green);
				}
			}
		}

		public float ObtainRadius(TutorialUnit unit) {
			if (unit == null) {
				return 0f;
			}
			Renderer renderer = unit.GetComponent<Renderer>();
			return renderer.bounds.extents.magnitude / 2f;
		}
	}
}
