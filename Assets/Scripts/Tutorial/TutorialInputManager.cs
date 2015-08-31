using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public class TutorialInputManager : MonoBehaviour {
		public List<GameObject> selectedObjects;
		public List<GameObject> boxSelectedObjects;

		public bool selectionTutorialFlag;
		public bool attackOrderTutorialFlag;
		public bool moveOrderTutorialFlag;
		public bool splitTutorialFlag;
		public bool mergeTutorialFlag;

		public bool attackStandingByFlag;
		public GameObject tutorialUnitPrefab;

		public TutorialAttackManager attackManager;
		public TutorialSplitManager splitManager;
		public TutorialMergeManager mergeManager;

		//----------------------------------

		void Start() {
			this.selectedObjects = new List<GameObject>();
			TutorialUnitManager.Instance.allObjects = new List<GameObject>();
			this.boxSelectedObjects = new List<GameObject>();

			//this.selectionTutorialFlag = this.attackOrderTutorialFlag = this.moveOrderTutorialFlag = this.splitTutorialFlag = this.mergeTutorialFlag = true;

			if (this.attackManager == null) {
				Debug.LogError("Cannot find attack manager for the tutorial.");
			}
			if (this.splitManager == null) {
				Debug.LogError("Cannot find split manager for the tutorial.");
			}

			GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("Tutorial_Unit");
			foreach (GameObject obj in existingObjects) {
				TutorialUnitManager.Instance.allObjects.Add(obj);
			}
		}

		void Update() {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				SelectOrder();
				MoveOrder();
				//AttackOrder();
				SplitOrder();
				MergeOrder();

				UpdateStatus();
			}
		}

		//----------------------------------

		private void UpdateStatus() {
			if (this.selectedObjects.Count > 0 || this.boxSelectedObjects.Count > 0) {
				if (this.attackStandingByFlag) {
					foreach (GameObject obj in TutorialUnitManager.Instance.allObjects) {
						if (this.selectedObjects.Contains(obj)) {
							TutorialUnit unit = obj.GetComponent<TutorialUnit>();
							if (!unit.isEnemy) {
								unit.SetAttackStandby();
							}
						}
					}
				}
				else {
					foreach (GameObject obj in TutorialUnitManager.Instance.allObjects) {
						if (obj == null) {
							TutorialUnitManager.Instance.removeList.Add(obj);
							continue;
						}
						if (this.selectedObjects.Contains(obj) || this.boxSelectedObjects.Contains(obj)) {
							TutorialUnit unit = obj.GetComponent<TutorialUnit>();
							if (unit == null) {
								TutorialUnitManager.Instance.removeList.Add(obj);
								continue;
							}
							if (!unit.isEnemy) {
								if (unit.isStandingBy) {
									unit.SetAttackStandby();
								}
								if (unit.isAttacking) {
									unit.SetAttack();
								}
								if (unit.isSplitting) {
									unit.SetDeselect();
									unit.SetAttackCancel();
								}
								if (unit.isSelected) {
									unit.SetSelect();
								}
							}
						}
					}
				}
			}
			else {
				foreach (GameObject obj in TutorialUnitManager.Instance.allObjects) {
					if (obj == null) {
						TutorialUnitManager.Instance.removeList.Add(obj);
						continue;
					}
					TutorialUnit unit = obj.GetComponent<TutorialUnit>();
					unit.SetDeselect();
				}
			}
		}

		//----------------------------------

		private void SelectOrder() {
			if (!this.selectionTutorialFlag) {
				return;
			}
			if (this.attackStandingByFlag) {
				return;
			}
			if (Input.GetMouseButtonDown(0)) {
				if (this.selectedObjects.Count > 0) {
					foreach (GameObject obj in this.selectedObjects) {
						TutorialUnit unit = obj.GetComponent<TutorialUnit>();
						unit.SetDeselect();
					}
					this.selectedObjects.Clear();
				}
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray);
				bool hasHitUnit = false;
				foreach (RaycastHit hit in hits) {
					GameObject obj = hit.collider.gameObject;
					if (obj.tag.Equals("Tutorial_Unit")) {
						hasHitUnit = true;
						if (!this.selectedObjects.Contains(obj)) {
							TutorialUnit unit = obj.GetComponent<TutorialUnit>();
							unit.SetSelect();
							this.selectedObjects.Add(obj);
						}
						break;
					}
				}
				if (!hasHitUnit) {
					this.selectedObjects.Clear();
				}
			}
			if (Input.GetMouseButton(0)) {
				foreach (GameObject obj in TutorialUnitManager.Instance.allObjects) {
					if (obj == null) {
						TutorialUnitManager.Instance.removeList.Add(obj);
						continue;
					}
					Vector2 screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
					screenPoint.y = Screen.height - screenPoint.y;
					if (Selection.selectionArea.Contains(screenPoint) && !this.boxSelectedObjects.Contains(obj)) {
						this.boxSelectedObjects.Add(obj);
						if (!this.selectedObjects.Contains(obj)) {
							this.selectedObjects.Add(obj);
						}
						TutorialUnit unit = obj.GetComponent<TutorialUnit>();
						unit.SetSelect();
					}
					else if (!Selection.selectionArea.Contains(screenPoint) && this.boxSelectedObjects.Contains(obj)) {
						this.boxSelectedObjects.Remove(obj);
						if (this.selectedObjects.Contains(obj)) {
							this.selectedObjects.Remove(obj);
						}
						TutorialUnit unit = obj.GetComponent<TutorialUnit>();
						unit.SetDeselect();
					}
				}
			}
			if (Input.GetMouseButtonUp(0)) {
				if (this.boxSelectedObjects.Count > 0) {
					foreach (GameObject obj in this.boxSelectedObjects) {
						if (!this.selectedObjects.Contains(obj)) {
							TutorialUnit unit = obj.GetComponent<TutorialUnit>();
							unit.SetSelect();
							this.selectedObjects.Add(obj);
						}
					}
					this.boxSelectedObjects.Clear();
				}
			}
		}

		//----------------------------------

		private void AttackOrder() {
			if (!this.attackOrderTutorialFlag) {
				return;
			}
			if (Input.GetKeyDown(KeyCode.A)) {
				if (!this.attackStandingByFlag) {
					if (this.selectedObjects.Count > 0) {
						this.attackStandingByFlag = true;
						foreach (GameObject obj in this.selectedObjects) {
							TutorialUnit unit = obj.GetComponent<TutorialUnit>();
							if (!unit.isEnemy) {
								unit.SetAttackStandby();
							}
						}
						//this.selectedObjects.Clear();
					}
				}
			}
			if (this.attackStandingByFlag) {
				if (Input.GetMouseButtonDown(0)) {
					this.attackStandingByFlag = false;
					foreach (GameObject obj in this.selectedObjects) {
						TutorialUnit unit = obj.GetComponent<TutorialUnit>();
						unit.SetAttackCancel();
					}
					this.selectedObjects.Clear();
				}
				else if (Input.GetMouseButtonDown(1)) {
					this.attackStandingByFlag = false;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit[] hits = Physics.RaycastAll(ray);
					//bool hasOrderedAttackTarget = false;
					foreach (RaycastHit hit in hits) {
						GameObject obj = hit.collider.gameObject;
						if (obj.name.Equals("Floor")) {
							foreach (GameObject selected in this.selectedObjects) {
								TutorialUnit unit = selected.GetComponent<TutorialUnit>();
								if (!unit.isEnemy) {
									unit.SetAttackCancel();
									unit.SetNewDestination(hit.point);
									unit.SetAttack();
								}
							}
							//hasOrderedAttackTarget = true;
							break;
						}
					}
					//if (hasOrderedAttackTarget) {
					//	this.selectedObjects.Clear();
					//}
				}
			}
		}

		//----------------------------------

		private void MoveOrder() {
			if (!this.moveOrderTutorialFlag) {
				return;
			}
			if (!this.attackStandingByFlag) {
				if (Input.GetMouseButtonDown(1)) {
					if (this.selectedObjects.Count > 0) {
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						RaycastHit[] hits = Physics.RaycastAll(ray);
						foreach (RaycastHit hit in hits) {
							GameObject obj = hit.collider.gameObject;
							if (obj.name.Equals("Floor")) {
								foreach (GameObject select in this.selectedObjects) {
									TutorialUnit unit = select.GetComponent<TutorialUnit>();
									if (!unit.isEnemy) {
										unit.SetAttackCancel();
										unit.SetNoEnemyTarget();
										unit.enemies.Clear();
										unit.SetStartMoving();
										unit.SetNewDestination(hit.point);
									}
								}
								break;
							}
						}
					}
					//this.selectedObjects.Clear();
				}
			}
		}

		//----------------------------------

		private void SplitOrder() {
			if (!this.splitTutorialFlag) {
				return;
			}
			if (Input.GetKeyDown(KeyCode.S)) {
				if (this.selectedObjects.Count > 0) {
					bool enemyCheck = false;
					foreach (GameObject obj in this.selectedObjects) {
						TutorialUnit unit = obj.GetComponent<TutorialUnit>();
						if (unit.isEnemy) {
							enemyCheck = true;
						}
					}
					if (!enemyCheck) {
						foreach (GameObject owner in this.selectedObjects) {
							TutorialUnit unit = owner.GetComponent<TutorialUnit>();
							GameObject duplicate = GameObject.Instantiate<GameObject>(this.tutorialUnitPrefab);
							duplicate.transform.position = owner.transform.position;
							unit.SetDeselect();
							unit.DisableSelection();
							unit.SetSplitting();
							unit = duplicate.GetComponent<TutorialUnit>();
							unit.SetDeselect();
							unit.DisableSelection();
							unit.SetSplitting();
							unit.initialColor = Color.white;
							TutorialUnitManager.Instance.allObjects.Add(duplicate);
							this.splitManager.splitGroups.Add(new SplitGroup(owner, duplicate));
						}
					}
					this.selectedObjects.Clear();
				}
			}
		}

		//----------------------------------

		private void MergeOrder() {
			if (!this.mergeTutorialFlag) {
				return;
			}
			if (Input.GetKeyDown(KeyCode.D)) {
				if (this.selectedObjects.Count > 0) {
					bool enemyCheck = false;
					foreach (GameObject obj in this.selectedObjects) {
						TutorialUnit unit = obj.GetComponent<TutorialUnit>();
						if (unit.isEnemy) {
							enemyCheck = true;
						}
					}
					if (!enemyCheck) {
						List<GameObject> pairedUnitsList = new List<GameObject>();
						for (int i = 0; i < this.selectedObjects.Count - 1; i++) {
							if (pairedUnitsList.Contains(this.selectedObjects[i])){
								continue;
							}
							TutorialUnit unpairedUnit = this.selectedObjects[i].GetComponent<TutorialUnit>();
							for (int j = i + 1; j < this.selectedObjects.Count; j++) {
								if (pairedUnitsList.Contains(this.selectedObjects[j])) {
									continue;
								}
								TutorialUnit undeterminedUnit = this.selectedObjects[j].GetComponent<TutorialUnit>();
								if (unpairedUnit.level == undeterminedUnit.level) {
									unpairedUnit.DisableSelection();
									unpairedUnit.SetDeselect();
									unpairedUnit.SetMerging();
									undeterminedUnit.DisableSelection();
									undeterminedUnit.SetDeselect();
									undeterminedUnit.SetMerging();
									this.mergeManager.mergeGroups.Add(new MergeGroup(this.selectedObjects[i], this.selectedObjects[j]));
									pairedUnitsList.Add(this.selectedObjects[i]);
									pairedUnitsList.Add(this.selectedObjects[j]);
									break;
								}
							}
						}
					}
					this.selectedObjects.Clear();
				}
			}
		}
	}
}
