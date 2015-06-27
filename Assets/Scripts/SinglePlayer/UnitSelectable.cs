using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSelectable : MonoBehaviour {
	public Color selectedColor = new Color(0f, 1f, 0f);
	public System.Guid identity = System.Guid.NewGuid();

	private Color initialColor;
	private UnitStateManager manager;

	private void Start() {
		Renderer renderer = this.GetComponent<Renderer>();
		this.initialColor = renderer.material.color;
		this.manager = this.GetComponent<UnitStateManager>();
		if (this.manager == null) {
			Debug.LogError(new System.NullReferenceException("There's something wrong with PlayerUnitManager."));
		}
	}

	private void Update() {
		if (Input.GetMouseButton(0)) {
			Select(this.manager);
		}
		else if (Input.GetMouseButtonUp(0)) {
			UpdatePlayerUnitManager(this.manager);
		}
	}

	private void SelectClick(UnitStateManager manager) {
		//This means unit is currently not within the selection box, check to see if it is being clicked on.
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject obj = hitInfo.collider.gameObject;
			Vector3 center = obj.transform.position + new Vector3(0f, 0.5f, 0f);
			float distance = Vector3.Distance(hitInfo.point, center);
			if (!obj.name.Equals("Floor")) {
				System.Guid identity = obj.GetComponent<UnitSelectable>().identity;
				if (distance <= 1f && identity.Equals(this.identity)) {
					manager.selectFlag = true;
				}
			}
		}
	}

	private void Select(UnitStateManager manager) {
		Vector3 worldToCamPosition = Camera.main.WorldToScreenPoint(this.transform.position);
		worldToCamPosition.y = Screen.height - worldToCamPosition.y;
		manager.selectFlag = Selection.selectionArea.Contains(worldToCamPosition);
		if (!manager.selectFlag) {
			SelectClick(manager);
		}
		RefreshSelectFlag(manager);
	}

	private void RefreshSelectFlag(UnitStateManager manager) {
		if (manager != null) {
			Renderer renderer = this.GetComponent<Renderer>();
			if (renderer != null) {
				if (manager.selectFlag) {
					renderer.material.color = this.selectedColor;
				}
				else {
					renderer.material.color = this.initialColor;
				}
			}
		}
	}

	private void UpdatePlayerUnitManager(UnitStateManager manager) {
		if (manager.selectFlag && !PlayerUnitManager.Instance.SelectableUnits.Contains(this.gameObject)) {
			PlayerUnitManager.Instance.SelectableUnits.Add(this.gameObject);
		}
		else if (!manager.selectFlag && PlayerUnitManager.Instance.SelectableUnits.Contains(this.gameObject)) {
			PlayerUnitManager.Instance.SelectableUnits.Remove(this.gameObject);
		}
	}
}
