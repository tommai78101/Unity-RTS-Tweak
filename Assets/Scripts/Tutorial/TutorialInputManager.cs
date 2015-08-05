using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialInputManager : MonoBehaviour {

	public List<GameObject> selectedObjects;
	public List<GameObject> allObjects;
	public List<GameObject> boxSelectedObjects;

	public bool selectionTutorialFlag;
	public bool attackOrderTutorialFlag;


	//----------------------------------


	void Start() {
		this.selectedObjects = new List<GameObject>();
		this.allObjects = new List<GameObject>();
		this.boxSelectedObjects = new List<GameObject>();

		GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("Tutorial_Unit");
		foreach (GameObject obj in existingObjects) {
			this.allObjects.Add(obj);
		}
	}

	void Update() {
		Select();
		AttackOrder();


		UpdateStatus();
	}

	//----------------------------------

	private void UpdateStatus() {
		foreach (GameObject obj in this.allObjects) {
			TutorialSelectable select = obj.GetComponent<TutorialSelectable>();
			if (this.selectedObjects.Contains(obj) || this.boxSelectedObjects.Contains(obj)) {
				select.SetSelect();
			}
			else {
				select.SetDeselect();
			}
		}
	}

	//----------------------------------

	private void Select() {
		if (!this.selectionTutorialFlag) {
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
		else if (Input.GetMouseButton(0)) {
			foreach (GameObject obj in this.allObjects) {
				Vector2 screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
				if (Selection.selectionArea.Contains(screenPoint) && !this.boxSelectedObjects.Contains(obj)) {
					this.boxSelectedObjects.Add(obj);
				}
				else if (!Selection.selectionArea.Contains(screenPoint) && this.boxSelectedObjects.Contains(obj)) {
					this.boxSelectedObjects.Remove(obj);
				}
			}
		}
		else if (Input.GetMouseButtonUp(0)) {
			if (this.boxSelectedObjects.Count > 0) {
				this.selectedObjects.AddRange(this.boxSelectedObjects);
				this.boxSelectedObjects.Clear();
			}
		}
	}

	private void AttackOrder() {
		if (!this.attackOrderTutorialFlag) {
			return;
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			//TODO
		}
	}
}
