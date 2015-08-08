using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Tutorial {
	public class TutorialSelectable : NetworkBehaviour {
		public Color selectionColor;
		public Color standbyColor;
		public bool isSelected;
		public bool isStandingBy;
		public bool isAttacking;
		public bool isSplitting;
		public bool canBeSelected;
		public Color initialColor;

		private void Start() {
			Renderer renderer = this.GetComponent<Renderer>();
			this.initialColor = renderer.material.color;
			if (this.initialColor.Equals(Color.black)) {
				this.initialColor = Color.white;
			}
			this.canBeSelected = true;
			TutorialUnitManager.Instance.allObjects.Add(this.gameObject);
		}

		public void SetSelect() {
			if (!this.canBeSelected) {
				return;
			}
			this.isSelected = true;
			if (this.isAttacking) {
				SetColor(this.initialColor);
			}
			else {
				SetColor(this.selectionColor);
			}
		}

		public void SetDeselect() {
			this.isSelected = false;
			SetColor(this.initialColor);
		}

		public void SetAttack() {
			this.isStandingBy = false;
			this.isAttacking = true;
			SetColor(this.initialColor);
		}

		public void SetAttackStandby() {
			this.isStandingBy = true;
			SetColor(this.standbyColor);
		}

		public void SetAttackCancel() {
			this.isStandingBy = false;
			this.isAttacking = false;
			SetColor(this.initialColor);
		}

		public void EnableSelection() {
			this.canBeSelected = true;
		}

		public void DisableSelection() {
			this.canBeSelected = false;
		}

		private void SetColor(Color newColor){
			Renderer renderer = this.GetComponent<Renderer>();
			renderer.material.color = newColor;
		}
	}
}
