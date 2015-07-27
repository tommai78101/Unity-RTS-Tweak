using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	public static List<Selectable> selectedObjects = new List<Selectable>();
	public bool isSelected = false;
	public bool isBoxSelected = false;
	public bool isEnabled;
	public Color selectedColor;
	public System.Guid UUID; 

	private int SelectableID = 0;
	private readonly Color initialColor = Color.white;
	private Attackable attackable; 
	private DeathCheck deathCheck;

	private void SelectAction(Color selectColor) {
		Renderer renderer = this.GetComponentInChildren<Renderer>();
		if (renderer.enabled && Input.GetMouseButton(0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(this.transform.position);
			camPos.y = Screen.height - camPos.y;
			this.isBoxSelected = Selection.selectionArea.Contains(camPos);
			if (!this.isBoxSelected) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit HitInfo;
				if (Physics.Raycast(ray, out HitInfo)) {
					GameObject obj = HitInfo.collider.gameObject;
					if (!obj.name.Equals("Floor") && !obj.name.EndsWith("Location")) {
						Selectable selectable = obj.GetComponent<Selectable>();
						if (selectable.SelectableID == this.SelectableID && selectable.UUID.Equals(this.UUID)) {
							this.isBoxSelected = true;
						}
					}
				}
			}
		}

		if (this.isBoxSelected) {
			if (Input.GetMouseButtonUp(0)) {
				this.Select();
			}
			if (this.selectedColor.Equals(null)) {
				Debug.Log("[Selectable] Color is null.");
				return;
			}
			if (!this.attackable.isReadyToAttack) {
				renderer.material.color = selectColor;
			}
		}
		else {
			if (Input.GetMouseButtonUp(0)) {
				this.Deselect();
			}
			if (!this.attackable.isReadyToAttack) {
				renderer.material.color = this.initialColor;
			}
		}

		if (this.isSelected) {
			if (!Selectable.selectedObjects.Contains(this)) {
				Selectable.selectedObjects.Add(this);
			}
		}
		else {
			if (Selectable.selectedObjects.Contains(this)) {
				Selectable.selectedObjects.Remove(this);
			}
		}
	}

	public void Awake() {
		this.UUID = System.Guid.NewGuid();
	}

	public void Deselect() {
		this.isSelected = false;
	}

	public void Select() {
		this.isSelected = true;
	}

	public void DisableSelection() {
		this.isEnabled = false;
		this.Deselect();
		this.isBoxSelected = false;
	}

	public void EnableSelection() {
		this.isEnabled = true;
	}

	public bool IsSelectionEnabled() {
		return this.isEnabled;
	}

	private void Start() {
		this.isEnabled = true;
		//NetworkView playerNetworkView = this.GetComponent<NetworkView>();
		//if (playerNetworkView != null && playerNetworkView.isMine) {
		//	this.isEnabled = true;
		//}

		this.attackable = this.GetComponent<Attackable>();
		if (this.attackable == null) {
			Debug.LogException(new System.NullReferenceException("Attackable is null."));
		}
		this.deathCheck = this.GetComponent<DeathCheck>();
		if (this.deathCheck == null) {
			Debug.LogException(new System.NullReferenceException("Death check is null."));
		}
	}

	private void Update() {
		if (!isEnabled) {
			if (!Debugging.debugEnabled && this.attackable.isReadyToAttack && !this.IsSelectionEnabled()) {
				this.EnableSelection();
			}
			else if (Selectable.selectedObjects.Contains(this)) {
				this.EnableSelection();
			}
			else if (!Debugging.debugEnabled && this.deathCheck.isDead) {
				this.DisableSelection();
			}
			return;
		}

		NetworkView networkView = this.GetComponent<NetworkView>();
		if (networkView != null) {
			if (networkView.isMine) {
				SelectAction(this.selectedColor);
			}
			else if (!networkView.isMine) {
				SelectAction(Color.magenta);
			}
		}
	}
}