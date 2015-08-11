using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public static class ExtensionClass {
		public static bool reachedDestination(this NavMeshAgent agent) {
			if (!agent.pathPending) {
				if (agent.remainingDistance <= agent.stoppingDistance) {
					if (!agent.hasPath || agent.velocity.sqrMagnitude <= float.Epsilon) {
						return true;
					}
				}
			}
			return false;
		}
	}

	public class TutorialUnit : MonoBehaviour {
		public int level;
		public int attackPower;
		public int maxHealth;
		public int currentHealth;
		public float fieldOfViewRadius;
		public float attackRadius;
		public float attackCooldown;
		public float attackCooldownTimer;
		public float damageCooldownTimer;
		public bool isEnemy;
		public bool isSelected;
		public bool isStandingBy;
		public bool isAttacking;
		public bool isSplitting;
		public bool isMerging;
		public bool isMoving;
		public bool canBeSelected;
		public bool isDead;
		public bool isTakingDamage;
		public Color selectionColor;
		public Color standbyColor;
		public Color initialColor;
		public List<TutorialUnit> enemies;
		public TutorialUnit enemyTarget;

		private void Start() {
			TutorialUnitManager.Instance.allObjects.Add(this.gameObject);

			Renderer renderer = this.GetComponent<Renderer>();
			this.initialColor = renderer.material.color;
			if (this.initialColor.Equals(Color.black)) {
				this.initialColor = Color.white;
			}
			Vector3 size = renderer.bounds.size;
			this.fieldOfViewRadius = 2.5f;
			this.attackRadius = Mathf.Ceil(((size / 2f).magnitude));
			if (this.attackCooldown <= 3f) {
				this.attackCooldown = 3f;
			}

			this.canBeSelected = true;
			this.level = 1;
			this.attackPower = 1;
			this.maxHealth = 5;
			this.currentHealth = 5;
			//this.isEnemy = false;
			this.isDead = false;
			this.isTakingDamage = false;

			this.enemies = new List<TutorialUnit>();
			this.enemyTarget = null;
		}

		private void Update() {
			if (!this.isDead) {
				NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
				if (!this.isTakingDamage) {
					if (this.isStandingBy || this.isAttacking) {
						SetColor(this.standbyColor);
					}
					else if (agent.reachedDestination() || this.isSplitting || this.isMerging) {
						SetColor(this.initialColor);
					}
				}

				if (this.isMoving) {
					SetAttackCancel();
					SetNoEnemyTarget();
					this.enemies.Clear();
					if (agent.reachedDestination()) {
						SetStopMoving();
					}
				}
				else if (this.isAttacking) {
					if (this.enemies.Count <= 0) {
						LocateEnemies();
					}
					if (this.enemies.Count > 0) {
						if (this.enemies[0] != null) {
							this.enemyTarget = this.enemies[0];
						}
					}
					if (this.enemyTarget != null && this.enemyTarget.isEnemy) {
						if (Vector3.Distance(this.transform.position, this.enemyTarget.transform.position) <= ObtainRadius(this) + this.attackRadius) {
							if (this.enemyTarget.currentHealth > 0) {
								SetAttack();
								if (this.attackCooldownTimer <= 0f) {
									this.attackCooldownTimer = this.attackCooldown;
									this.enemyTarget.TakeDamage(this.attackPower);
								}
								else {
									this.attackCooldownTimer -= Time.deltaTime;
								}
							}
						}
						else {
							agent.stoppingDistance = ObtainRadius(this.enemyTarget) + this.attackRadius;
							agent.SetDestination(this.enemyTarget.transform.position);
						}
					}
					else {
						if (agent.reachedDestination()) {
							SetAttackCancel();
							//SetSelect();
						}
					}
				}
				else {
					LocateEnemies();
					if (this.enemies.Count > 0) {
						if (this.enemies[0] != null) {
							this.enemyTarget = this.enemies[0];
							if (this.enemyTarget.currentHealth > 0) {
								Vector3 enemyPosition = this.enemies[0].transform.position;
								if (Vector3.Distance(this.transform.position, enemyPosition) <= ObtainRadius(this) + this.fieldOfViewRadius) {
									agent.stoppingDistance = ObtainRadius(this.enemyTarget) + this.attackRadius;
									agent.SetDestination(this.enemyTarget.transform.position);
									if (!this.isAttacking) {
										SetAttack();
									}
								}
								else {
									if (this.isAttacking) {
										SetAttackCancel();
									}
								}
							}
						}
						else {
							this.enemies.RemoveAt(0);
						}
					}
					else {
						SetNoEnemyTarget();
					}
				}

				if (this.damageCooldownTimer > 0f) {
					SetColor(Color.Lerp(this.initialColor, Color.red, this.damageCooldownTimer));
					this.damageCooldownTimer -= Time.deltaTime;
				}
				if (this.currentHealth <= 0) {
					this.isDead = true;
					TutorialUnitManager.Instance.removeList.Add(this.gameObject);
				}
			}
		}

		public void SetSelect() {
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

		public void SetDeselect() {
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

		public void SetStartMoving() {
			this.isMoving = true;
		}

		public void SetStopMoving() {
			this.isMoving = false;
		}

		public void SetAttack() {
			this.isStandingBy = false;
			this.isAttacking = true;
		}

		public void SetAttackStandby() {
			this.isStandingBy = true;
		}

		public void SetAttackCancel() {
			this.isStandingBy = false;
			this.isAttacking = false;
		}

		public void SetNoEnemyTarget() {
			this.enemyTarget = null;
		}

		public void SetNewDestination(Vector3 point) {
			NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
			agent.stoppingDistance = 0f;
			agent.SetDestination(point);
		}

		public void EnableSelection() {
			this.canBeSelected = true;
		}

		public void DisableSelection() {
			this.canBeSelected = false;
		}

		public void SetColor(Color newColor) {
			Renderer renderer = this.GetComponent<Renderer>();
			renderer.material.color = newColor;
		}

		public void SetEnemyFlag(bool value) {
			this.isEnemy = value;
		}

		public void SetMerging() {
			this.isMerging = true;
		}

		public void SetNotMerging() {
			this.isMerging = false;
		}

		public void SetSplitting() {
			this.isSplitting = true;
		}

		public void SetNotSplitting() {
			this.isSplitting = false;
		}

		public void TakeDamage(int damage) {
			if (this.currentHealth > 0) {
				this.currentHealth -= damage;
			}
			if (this.damageCooldownTimer <= 0f) {
				this.damageCooldownTimer = 1f;
				this.isTakingDamage = true;
			}
			else {
				this.isTakingDamage = false;
			}
		}

		public void LocateEnemies() {
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.fieldOfViewRadius + ObtainRadius(this));
			if (colliders.Length > 0) {
				foreach (Collider col in colliders) {
					TutorialUnit unit = col.GetComponent<TutorialUnit>();
					if (unit == this) {
						continue;
					}
					if (unit != null) {
						if (unit.isEnemy) {
							if (!this.enemies.Contains(unit)) {
								this.enemies.Add(unit);
							}
						}
					}
				}
			}
		}

		//-------------------------------------------

		private float ObtainRadius(TutorialUnit unit) {
			if (unit == null) {
				return 0f;
			}
			Renderer renderer = unit.GetComponent<Renderer>();
			return renderer.bounds.extents.magnitude / 2f;
		}
	}
}
