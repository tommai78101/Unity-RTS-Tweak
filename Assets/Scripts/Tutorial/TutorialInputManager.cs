using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public class TutorialInputManager : MonoBehaviour {

		public List<GameObject> selectedObjects;
		public List<GameObject> allObjects;
		public List<GameObject> boxSelectedObjects;

		public bool selectionTutorialFlag;
		public bool attackOrderTutorialFlag;
		public bool moveOrderTutorialFlag;
		public bool splitTutorialFlag;

		public bool attackStandingByFlag;
		public GameObject tutorialUnitPrefab;

		public TutorialAttackManager attackManager;
		public TutorialSplitManager splitManager;


		//----------------------------------


		void Start() {
			this.selectedObjects = new List<GameObject>();
			this.allObjects = new List<GameObject>();
			this.boxSelectedObjects = new List<GameObject>();

			this.selectionTutorialFlag = this.attackOrderTutorialFlag = this.moveOrderTutorialFlag = this.splitTutorialFlag = true;

			if (this.attackManager == null) {
				Debug.LogError("Cannot find attack manager for the tutorial.");
			}
			if (this.splitManager == null) {
				Debug.LogError("Cannot find split manager for the tutorial.");
			}

			GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("Tutorial_Unit");
			foreach (GameObject obj in existingObjects) {
				this.allObjects.Add(obj);
			}
		}

		void Update() {
			SelectOrder();
			AttackOrder();
			MoveOrder();
			SplitOrder();

			UpdateStatus();
		}

		//----------------------------------

		private void UpdateStatus() {
			if (this.attackStandingByFlag) {
				foreach (GameObject obj in this.allObjects) {
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
				foreach (GameObject obj in this.allObjects) {
					TutorialSelectable select = obj.GetComponent<TutorialSelectable>();
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
				foreach (GameObject obj in this.allObjects) {
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
					foreach (RaycastHit hit in hits) {
						GameObject obj = hit.collider.gameObject;
						if (obj.name.Equals("Floor")) {
							AttackOrder order = new AttackOrder();
							order.Create(hit.point, this.selectedObjects);
							this.attackManager.attackOrders.Add(order);
							break;
						}
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
				if (this.selectedObjects.Count > 0) {
					if (Input.GetMouseButtonDown(1)) {
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
				}
			}
		}

		//----------------------------------

		private void SplitOrder() {
			if (!this.splitTutorialFlag) {
				return;
			}
			if (this.selectedObjects.Count > 0) {
				if (Input.GetKeyDown(KeyCode.S)) {
					foreach (GameObject owner in this.selectedObjects) {
						GameObject duplicate = GameObject.Instantiate<GameObject>(this.tutorialUnitPrefab);
						duplicate.transform.position = owner.transform.position;
						TutorialSelectable select = owner.GetComponent<TutorialSelectable>();
						select.DisableSelection();
						select = duplicate.GetComponent<TutorialSelectable>();
						select.DisableSelection();
						this.allObjects.Add(duplicate);
						this.splitManager.splitGroups.Add(new SplitGroup(owner, duplicate));
					}
					this.selectedObjects.Clear();
				}
			}
		}
	}
}
