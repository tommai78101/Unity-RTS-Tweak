using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public class TutorialUnit: MonoBehaviour {
		public int level;
		public int attackPower;
		public int maxHealth;
		public int currentHealth;
		public float attackRadius;
		public bool isEnemy;
		public Color selectionColor;
		public Color standbyColor;
		public bool isSelected;
		public bool isStandingBy;
		public bool isAttacking;
		public bool isSplitting;
		public bool canBeSelected;
		public Color initialColor;
		public List<TutorialUnit> enemies;

		private void Start() {
			TutorialUnitManager.Instance.allObjects.Add(this.gameObject);

			Renderer renderer = this.GetComponent<Renderer>();
			this.initialColor = renderer.material.color;
			if (this.initialColor.Equals(Color.black)) {
				this.initialColor = Color.white;
			}
			Vector3 size = renderer.bounds.size;
			this.attackRadius = 2.5f + ((size / 2f).magnitude);

			this.canBeSelected = true;
			this.level = 1;
			this.attackPower = 1;
			this.maxHealth = 5;
			this.currentHealth = 5;
			this.isEnemy = false;

			this.enemies = new List<TutorialUnit>();
		}

		private void Update() {

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

		public void SetColor(Color newColor){
			Renderer renderer = this.GetComponent<Renderer>();
			renderer.material.color = newColor;
		}

		public void SetEnemyFlag(bool value) {
			this.isEnemy = value;
		}

		public void LocateEnemies() {
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.attackRadius);
			if (colliders.Length > 0) {
				foreach (Collider col in colliders) {
					TutorialUnit unit = col.GetComponent<TutorialUnit>();
					if (unit != null) {
						this.enemies.Add(unit);
					}
				}
			}
		}
	}
}
