using UnityEngine;
using System.Collections;

namespace Tutorial {
	public class TutorialSelectable : MonoBehaviour {
		public Color selectionColor;
		public Color standbyColor;
		public bool isSelected;
		public bool isStandingBy;
		public bool isAttacking;

		private Color initialColor;

		private void Start() {
			Renderer renderer = this.GetComponent<Renderer>();
			this.initialColor = renderer.material.color;
		}

		public void SetSelect() {
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

		private void SetColor(Color newColor){
			Renderer renderer = this.GetComponent<Renderer>();
			renderer.material.color = newColor;
		}
	}
}
