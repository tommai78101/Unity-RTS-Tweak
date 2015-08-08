using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public class TutorialInputManager : NetworkBehaviour {
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

			this.selectionTutorialFlag = this.attackOrderTutorialFlag = this.moveOrderTutorialFlag = this.splitTutorialFlag = this.mergeTutorialFlag = true;

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
			SelectOrder();
			AttackOrder();
			MoveOrder();
			SplitOrder();
			MergeOrder();

			UpdateStatus();
		}

		//----------------------------------

		private void UpdateStatus() {
			if (this.attackStandingByFlag) {
				foreach (GameObject obj in TutorialUnitManager.Instance.allObjects) {
					TutorialSelectable select = obj.GetComponent<TutorialSelectable>();
					if (this.selectedObjects.Contains(obj)) {
						select.SetAttackStandby();
					}
					else {
						select.SetDeselect();
					}
				}
			}
			else {
				foreach (GameObject obj in TutorialUnitManager.Instance.allObjects) {
					if (obj == null) {
						TutorialUnitManager.Instance.removeList.Add(obj);
						continue;
					}
					TutorialSelectable select = obj.GetComponent<TutorialSelectable>();
					if (select == null) {
						TutorialUnitManager.Instance.removeList.Add(obj);
						continue;
					}
					if (this.selectedObjects.Contains(obj) || this.boxSelectedObjects.Contains(obj)) {
						select.SetSelect();
					}
					else {
						select.SetDeselect();
						select.SetAttackCancel();
					}
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
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll(ray);
				bool hasHitUnit = false;
				foreach (RaycastHit hit in hits) {
					GameObject obj = hit.collider.gameObject;
					if (obj.tag.Equals("Tutorial_Unit")) {
						hasHitUnit = true;
						if (!this.selectedObjects.Contains(obj)) {
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
					Vector2 screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
					screenPoint.y = Screen.height - screenPoint.y;
					if (Selection.selectionArea.Contains(screenPoint) && !this.boxSelectedObjects.Contains(obj)) {
						this.boxSelectedObjects.Add(obj);
					}
					else if (!Selection.selectionArea.Contains(screenPoint) && this.boxSelectedObjects.Contains(obj)) {
						this.boxSelectedObjects.Remove(obj);
					}
				}
			}
			if (Input.GetMouseButtonUp(0)) {
				if (this.boxSelectedObjects.Count > 0) {
					foreach (GameObject obj in this.boxSelectedObjects) {
						if (!this.selectedObjects.Contains(obj)) {
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
							TutorialSelectable select = obj.GetComponent<TutorialSelectable>();
							select.SetAttackStandby();
						}
						this.selectedObjects.Clear();
					}
				}
			}
			if (this.attackStandingByFlag) {
				if (Input.GetMouseButtonDown(0)) {
					this.attackStandingByFlag = false;
					this.selectedObjects.Clear();
				}
				else if (Input.GetMouseButtonDown(1)) {
					this.attackStandingByFlag = false;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit[] hits = Physics.RaycastAll(ray);
					bool hasOrderedAttackTarget = false;
					foreach (RaycastHit hit in hits) {
						GameObject obj = hit.collider.gameObject;
						if (obj.name.Equals("Floor")) {
							AttackOrder order = new AttackOrder();
							order.Create(hit.point, this.selectedObjects);
							this.attackManager.attackOrders.Add(order);
							hasOrderedAttackTarget = true;
							break;
						}
					}
					if (hasOrderedAttackTarget) {
						this.selectedObjects.Clear();
					}
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
									NavMeshAgent agent = select.GetComponent<NavMeshAgent>();
									agent.SetDestination(hit.point);
								}
								break;
							}
						}
					}
					this.selectedObjects.Clear();
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
					foreach (GameObject owner in this.selectedObjects) {
						GameObject duplicate = GameObject.Instantiate<GameObject>(this.tutorialUnitPrefab);
						duplicate.transform.position = owner.transform.position;
						TutorialSelectable select = owner.GetComponent<TutorialSelectable>();
						select.DisableSelection();
						select = duplicate.GetComponent<TutorialSelectable>();
						select.DisableSelection();
						select.initialColor = Color.white;
						TutorialUnitManager.Instance.allObjects.Add(duplicate);
						this.splitManager.splitGroups.Add(new SplitGroup(owner, duplicate));
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
					for (int i = 0; i < this.selectedObjects.Count && (i + 1 < this.selectedObjects.Count); i += 2) {
						TutorialSelectable select = this.selectedObjects[i].GetComponent<TutorialSelectable>();
						select.DisableSelection();
						select = this.selectedObjects[i + 1].GetComponent<TutorialSelectable>();
						select.DisableSelection();
						this.mergeManager.mergeGroups.Add(new MergeGroup(this.selectedObjects[i], this.selectedObjects[i + 1]));
					}
					this.selectedObjects.Clear();
				}
			}
		}
	}
}
