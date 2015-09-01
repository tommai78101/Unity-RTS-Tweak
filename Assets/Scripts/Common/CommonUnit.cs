using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Extension;

namespace Common {
	public class CommonUnit : NetworkBehaviour {
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

		public List<CommonUnit> enemies;
		public CommonUnit enemyTarget;
		public CommonUnitManager unitManager;

		// Use this for initialization
		protected void Start() {


			GameObject gameObject = GameObject.Find("Unit Manager");
			if (gameObject != null) {
				this.unitManager = gameObject.GetComponent<CommonUnitManager>();
				if (this.unitManager == null) {
					Debug.LogError("Common: Unit Manager is not created in the editor.");
				}
				else {
					this.unitManager.getAllObjects().Add(this.gameObject);
				}
			}

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

			this.enemies = new List<CommonUnit>();
			this.enemyTarget = null;
		}

		// Update is called once per frame
		protected void Update() {
			if (!this.isDead) {
				ActionTick();
				IdleActions();

				if (this.damageCooldownTimer > 0f) {
					SetColor(Color.Lerp(this.initialColor, Color.red, this.damageCooldownTimer));
					this.damageCooldownTimer -= Time.deltaTime;
				}
				if (this.currentHealth <= 0) {
					this.isDead = true;
					this.unitManager.getRemoveList().Add(this.gameObject);
				}
				if (this.attackCooldownTimer < 0f) {
					if (this.isAttacking) {
						this.attackCooldownTimer = this.attackCooldown;
					}
				}
				else {
					this.attackCooldownTimer -= Time.deltaTime;
				}
			}
		}

		public void ActionTick() {
			if (!this.isTakingDamage) {
				if (this.isMoving || this.isAttacking) {
					NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
					if (agent.ReachedDestination()) {
						SetStopMoving();
					}
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
									this.enemyTarget.TakeDamage(this.attackPower);
								}
							}
						}
						else {
							agent.stoppingDistance = ObtainRadius(this.enemyTarget) + this.attackRadius;
							agent.SetDestination(this.enemyTarget.transform.position);
						}
					}
					else {
						if (agent.ReachedDestination()) {
							SetAttackCancel();
							if (this.isSelected) {
								SetSelect();
							}
						}
					}
				}
			}
		}

		public void IdleActions() {
			LocateEnemies();
			if (this.enemies.Count > 0) {
				if (this.enemies[0] != null) {
					this.enemyTarget = this.enemies[0];
					if (this.enemyTarget.currentHealth > 0) {
						Vector3 enemyPosition = this.enemies[0].transform.position;
						if (Vector3.Distance(this.transform.position, enemyPosition) <= ObtainRadius(this) + this.fieldOfViewRadius) {
							NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
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

		public virtual void SetSelect() {
			if (!this.canBeSelected) {
				return;
			}
			this.isSelected = true;
			SelectionRing ring = this.GetComponentInChildren<SelectionRing>();
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

		public virtual void SetDeselect() {
			if (!this.canBeSelected) {
				return;
			}
			this.isSelected = false;
			SelectionRing ring = this.GetComponentInChildren<SelectionRing>();
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
			this.isMoving = true;
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
			if (this.isMoving) {
				return;
			}
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.fieldOfViewRadius + ObtainRadius(this));
			if (colliders.Length > 0) {
				foreach (Collider col in colliders) {
					CommonUnit unit = col.GetComponent<CommonUnit>();
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

		public void MultiplyAttributes(int value) {
			this.currentHealth *= value;
			this.maxHealth *= value;
			this.attackRadius *= value;
			this.attackPower *= value;
			this.attackCooldown *= (value * 0.4f);
			this.fieldOfViewRadius *= 1.2f;
			this.level++;
		}

		public float ObtainRadius(CommonUnit unit) {
			if (unit == null) {
				return 0f;
			}
			Renderer renderer = unit.GetComponent<Renderer>();
			return renderer.bounds.extents.magnitude / 2f;
		}
	}
}
