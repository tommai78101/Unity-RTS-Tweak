using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Extension;

namespace Common {
	public class CommonInputManager : NetworkBehaviour {
		[SerializeField]
		protected List<GameObject> selectedObjects;
		[SerializeField]
		protected List<GameObject> boxSelectedObjects;

		public CommonAttackManager attackManager;
		public CommonSplitManager splitManager;
		public CommonMergeManager mergeManager;
		public CommonUnitManager unitManager;

		//public bool attackStandingByFlag;
		public GameObject commonUnitPrefab;
		public string unitTagName;

		protected void Start() {
			if (this.attackManager == null) {
				Debug.LogError("Common: Cannot find attack manager.");
			}
			if (this.splitManager == null) {
				Debug.LogError("Common: Cannot find split manager.");
			}
			if (this.mergeManager == null) {
				Debug.LogError("Common: Cannot find merge manager.");
			}
			if (this.unitManager == null) {
				Debug.LogError("Common: Cannot find unit manager.");
			}
			if (this.unitTagName.IsEmpty()) {
				Debug.LogError("Common: Unit Tag Name is not set.");
			}

			this.selectedObjects = new List<GameObject>();
			this.boxSelectedObjects = new List<GameObject>();
		}

		protected virtual void Update() {
			SelectOrder();
			MoveOrder();
			SplitOrder();
			MergeOrder();

			UpdateStatus();
		}

		//----------------------------------

		protected virtual void UpdateStatus() {
			if (this.selectedObjects.Count > 0 || this.boxSelectedObjects.Count > 0) {
				foreach (GameObject obj in this.unitManager.getAllObjects()) {
					if (obj == null) {
						this.unitManager.getRemoveList().Add(obj);
						continue;
					}
					if (this.selectedObjects.Contains(obj) || this.boxSelectedObjects.Contains(obj)) {
						CommonUnit unit = obj.GetComponent<CommonUnit>();
						if (unit == null) {
							this.unitManager.getRemoveList().Add(obj);
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
			else {
				foreach (GameObject obj in this.unitManager.getAllObjects()) {
					if (obj == null) {
						this.unitManager.getRemoveList().Add(obj);
						continue;
					}
					CommonUnit unit = obj.GetComponent<CommonUnit>();
					unit.SetDeselect();
				}
			}
		}

		//----------------------------------

		protected virtual void SelectOrder() {
			if (Input.GetMouseButtonDown(0)) {
				if (this.selectedObjects.Count > 0) {
					for (int i = 0; i < this.selectedObjects.Count; i++) {
						GameObject obj = this.selectedObjects[i];
						if (obj == null) {
							this.selectedObjects.RemoveAt(i);
						}
						else {
							CommonUnit unit = obj.GetComponent<CommonUnit>();
							unit.SetDeselect();
						}
					}
					this.selectedObjects.Clear();
				}
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray);
				bool hasHitUnit = false;
				foreach (RaycastHit hit in hits) {
					GameObject obj = hit.collider.gameObject;
					//Units of type CommonUnit must have this tag set in the editor.
					if (obj.tag.Equals(this.unitTagName)) {
						hasHitUnit = true;
						if (!this.selectedObjects.Contains(obj)) {
							CommonUnit unit = obj.GetComponent<CommonUnit>();
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
				foreach (GameObject obj in this.unitManager.getAllObjects()) {
					if (obj == null) {
						this.unitManager.getRemoveList().Add(obj);
						continue;
					}
					Vector2 screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
					screenPoint.y = Screen.height - screenPoint.y;
					if (Selection.selectionArea.Contains(screenPoint) && !this.boxSelectedObjects.Contains(obj)) {
						this.boxSelectedObjects.Add(obj);
						if (!this.selectedObjects.Contains(obj)) {
							this.selectedObjects.Add(obj);
						}
						CommonUnit unit = obj.GetComponent<CommonUnit>();
						unit.SetSelect();
					}
					else if (!Selection.selectionArea.Contains(screenPoint) && this.boxSelectedObjects.Contains(obj)) {
						this.boxSelectedObjects.Remove(obj);
						if (this.selectedObjects.Contains(obj)) {
							this.selectedObjects.Remove(obj);
						}
						CommonUnit unit = obj.GetComponent<CommonUnit>();
						unit.SetDeselect();
					}
				}
			}
			if (Input.GetMouseButtonUp(0)) {
				if (this.boxSelectedObjects.Count > 0) {
					foreach (GameObject obj in this.boxSelectedObjects) {
						if (!this.selectedObjects.Contains(obj)) {
							CommonUnit unit = obj.GetComponent<CommonUnit>();
							unit.SetSelect();
							this.selectedObjects.Add(obj);
						}
					}
					this.boxSelectedObjects.Clear();
				}
			}
		}

		//----------------------------------

		//protected void AttackOrder() {
		//	if (Input.GetKeyDown(KeyCode.A)) {
		//		if (!this.attackStandingByFlag) {
		//			if (this.selectedObjects.Count > 0) {
		//				this.attackStandingByFlag = true;
		//				foreach (GameObject obj in this.selectedObjects) {
		//					CommonUnit unit = obj.GetComponent<CommonUnit>();
		//					if (!unit.isEnemy) {
		//						unit.SetAttackStandby();
		//					}
		//				}
		//				//this.selectedObjects.Clear();
		//			}
		//		}
		//	}
		//	if (this.attackStandingByFlag) {
		//		if (Input.GetMouseButtonDown(0)) {
		//			this.attackStandingByFlag = false;
		//			foreach (GameObject obj in this.selectedObjects) {
		//				CommonUnit unit = obj.GetComponent<CommonUnit>();
		//				unit.SetAttackCancel();
		//			}
		//			this.selectedObjects.Clear();
		//		}
		//		else if (Input.GetMouseButtonDown(1)) {
		//			this.attackStandingByFlag = false;
		//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//			RaycastHit[] hits = Physics.RaycastAll(ray);
		//			//bool hasOrderedAttackTarget = false;
		//			foreach (RaycastHit hit in hits) {
		//				GameObject obj = hit.collider.gameObject;
		//				if (obj.name.Equals("Floor")) {
		//					foreach (GameObject selected in this.selectedObjects) {
		//						CommonUnit unit = selected.GetComponent<CommonUnit>();
		//						if (!unit.isEnemy) {
		//							unit.SetAttackCancel();
		//							unit.SetNewDestination(hit.point);
		//							unit.SetAttack();
		//						}
		//					}
		//					//hasOrderedAttackTarget = true;
		//					break;
		//				}
		//			}
		//			//if (hasOrderedAttackTarget) {
		//			//	this.selectedObjects.Clear();
		//			//}
		//		}
		//	}
		//}

		//----------------------------------

		protected void MoveOrder() {
			if (Input.GetMouseButtonDown(1)) {
				if (this.selectedObjects.Count > 0) {
					for (int i = 0; i < this.selectedObjects.Count; i++) {
						if (this.selectedObjects[i] == null) {
							this.selectedObjects.RemoveAt(i);
						}
					}
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit[] hits = Physics.RaycastAll(ray);
					foreach (RaycastHit hit in hits) {
						GameObject obj = hit.collider.gameObject;
						if (obj.name.Equals("Floor")) {
							foreach (GameObject select in this.selectedObjects) {
								//CommonUnit unit = select.GetComponent<CommonUnit>();
								//if (!unit.isEnemy) {
								//	unit.SetAttackCancel();
								//	unit.SetNoEnemyTarget();
								//	unit.enemies.Clear();
								//	unit.SetStartMoving();
								//	unit.SetNewDestination(hit.point);
								//}
								CmdMoveOrder(select, hit.point);
							}
							break;
						}
					}
				}
				//this.selectedObjects.Clear();
			}
		}

		[Command]
		public void CmdMoveOrder(GameObject obj, Vector3 targetPoint) {
			ServerMoveOrder(obj, targetPoint);
		}

		[ClientCallback]
		public void ServerMoveOrder(GameObject obj, Vector3 targetPoint) {
			CommonUnit unit = obj.GetComponent<CommonUnit>();
			if (!unit.isEnemy) {
				unit.SetAttackCancel();
				unit.SetNoEnemyTarget();
				unit.enemies.Clear();
				unit.SetStartMoving();
				unit.SetNewDestination(targetPoint);
			}
		}

		//----------------------------------

		protected void SplitOrder() {
			if (Input.GetKeyDown(KeyCode.S)) {
				if (this.selectedObjects.Count > 0) {
					bool enemyCheck = false;
					foreach (GameObject obj in this.selectedObjects) {
						CommonUnit unit = obj.GetComponent<CommonUnit>();
						if (unit.isEnemy) {
							enemyCheck = true;
						}
					}
					if (!enemyCheck) {
						foreach (GameObject owner in this.selectedObjects) {
							CommonUnit unit = owner.GetComponent<CommonUnit>();
							if (unit.level == 1) {
								unit.SetDeselect();
								unit.DisableSelection();
								unit.SetSplitting();
								GameObject duplicate = GameObject.Instantiate(this.commonUnitPrefab, owner.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
								unit = duplicate.GetComponent<CommonUnit>();
								unit.SetDeselect();
								unit.DisableSelection();
								unit.SetSplitting();
								unit.initialColor = Color.white;
								NetworkServer.Spawn(duplicate);
								this.unitManager.getAllObjects().Add(duplicate);
								this.splitManager.splitGroups.Add(new SplitGroup(owner, duplicate));
							}
						}
					}
					this.selectedObjects.Clear();
				}
			}
		}

		//----------------------------------

		protected void MergeOrder() {
			if (Input.GetKeyDown(KeyCode.D)) {
				if (this.selectedObjects.Count > 0) {
					bool enemyCheck = false;
					foreach (GameObject obj in this.selectedObjects) {
						CommonUnit unit = obj.GetComponent<CommonUnit>();
						if (unit.isEnemy) {
							enemyCheck = true;
						}
					}
					if (!enemyCheck) {
						List<GameObject> pairedUnitsList = new List<GameObject>();
						for (int i = 0; i < this.selectedObjects.Count - 1; i++) {
							if (pairedUnitsList.Contains(this.selectedObjects[i])) {
								continue;
							}
							CommonUnit unpairedUnit = this.selectedObjects[i].GetComponent<CommonUnit>();
							for (int j = i + 1; j < this.selectedObjects.Count; j++) {
								if (pairedUnitsList.Contains(this.selectedObjects[j])) {
									continue;
								}
								CommonUnit undeterminedUnit = this.selectedObjects[j].GetComponent<CommonUnit>();
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
