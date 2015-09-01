using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace Tutorial {
	public class TutorialInputManager : CommonInputManager {
		public bool selectionTutorialFlag;
		public bool attackOrderTutorialFlag;
		public bool moveOrderTutorialFlag;
		public bool splitTutorialFlag;
		public bool mergeTutorialFlag;

		//----------------------------------

		protected new void Start() {
			base.Start();

			//this.selectionTutorialFlag = this.attackOrderTutorialFlag = this.moveOrderTutorialFlag = this.splitTutorialFlag = this.mergeTutorialFlag = true;

			if (this.attackManager == null) {
				Debug.LogError("Tutorial: Cannot find attack manager for the tutorial.");
			}
			if (this.splitManager == null) {
				Debug.LogError("Tutorial: Cannot find split manager for the tutorial.");
			}

			TutorialUnitManager.Instance.InitializeObjectList();
			GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("Tutorial_Unit");
			foreach (GameObject obj in existingObjects) {
				TutorialUnitManager.Instance.getAllObjects().Add(obj);
			}
		}

		protected override void Update() {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				SelectOrder();
				MoveOrder();
				SplitOrder();
				MergeOrder();

				base.UpdateStatus();
			}
		}

		protected override void SelectOrder() {
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
					if (obj.tag.Equals("Tutorial_Unit")) {   //<-----------  Fill in the unit's tag name here from the editor.
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
				foreach (GameObject obj in CommonUnitManager.Instance.getAllObjects()) {
					if (obj == null) {
						CommonUnitManager.Instance.getRemoveList().Add(obj);
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

		protected new void AttackOrder() {
			if (!this.attackOrderTutorialFlag) {
				return;
			}
			base.AttackOrder();
		}


		protected new void MoveOrder() {
			if (!this.moveOrderTutorialFlag) {
				return;
			}
			base.MoveOrder();
		}


		protected new void SplitOrder() {
			if (!this.splitTutorialFlag) {
				return;
			}
			base.SplitOrder();
		}

		protected new void MergeOrder() {
			if (!this.mergeTutorialFlag) {
				return;
			}
			base.MergeOrder();
		}
	}
}
