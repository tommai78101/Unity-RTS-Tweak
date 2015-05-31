using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selectable : MonoBehaviour {
	public static List<Selectable> selectedObjects = new List<Selectable>();
	public bool isSelected = false;
	public Color initialColor = Color.white;
	public Color selectedColor;
	public int SelectableID = 0;
	public System.Guid UUID = System.Guid.NewGuid();

	private bool isBoxedSelected = false;
	private bool isEnabled;

	[RPC]
	public void RPC_Select() {
		Renderer renderer = this.GetComponentInChildren<Renderer>();
		if (renderer.enabled && Input.GetMouseButton(0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(this.transform.position);
			camPos.y = Screen.height - camPos.y;
			this.isBoxedSelected = Selection.selectionArea.Contains(camPos);
			if (!this.isBoxedSelected) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit HitInfo;
				if (Physics.Raycast(ray, out HitInfo)) {
					GameObject obj = HitInfo.collider.gameObject;
					Vector3 center = obj.transform.position + new Vector3(0f, 0.5f, 0f);
					float distance = Vector3.Distance(HitInfo.point, center);
					Selectable selectable = obj.GetComponent<Selectable>();
					//Debug.Log("Vector distance from " + HitInfo.point.ToString() + " to " + center.ToString() + " is: " + distance.ToString());
					if (!obj.name.Equals("Floor") && !obj.name.EndsWith("Location")){
						if (distance <= 1f && selectable.SelectableID == this.SelectableID && selectable.UUID.Equals(this.UUID)) {
							this.isBoxedSelected = true;
						}
					}
				}
			}
		}

		if (this.isBoxedSelected) {
			if (this.selectedColor.Equals(null)) {
				Debug.Log("[Selectable] Color is null.");
				return;
			}
			renderer.material.color = this.selectedColor;
			if (Input.GetMouseButtonUp(0)) {
				this.isSelected = true;
				if (!Selectable.selectedObjects.Contains(this)) {
					Selectable.selectedObjects.Add(this);
				}
			}
		}
		else {
			if (Input.GetMouseButtonUp(0)) {
				this.isSelected = false;
			}
			renderer.material.color = this.initialColor;
		}

		if (Input.GetMouseButtonUp(0)) {
			while (!this.isSelected && Selectable.selectedObjects.Contains(this)) {
				Selectable.selectedObjects.Remove(this);
			}
		}
	}

	private void Start() {
		this.isEnabled = false;
		NetworkView playerNetworkView = this.GetComponent<NetworkView>();
		if (playerNetworkView != null && playerNetworkView.isMine) {
			this.isEnabled = true;
		}
	}

	private void Update() {
		if (!isEnabled) {
			return;
		}

		//NetworkView networkView = this.GetComponent<NetworkView>();
		//if (networkView != null) {
		//	networkView.RPC("RPC_Select", RPCMode.AllBuffered, null);
		//}

		#region UNWANTED_COMMENTS
		//Renderer renderer = this.GetComponentInChildren<Renderer>();
		//if (renderer.enabled && Input.GetMouseButton(0)) {
		//	Vector3 camPos = Camera.main.WorldToScreenPoint(this.transform.position);
		//	camPos.y = Screen.height - camPos.y;
		//	this.isBoxedSelected = Selection.selectionArea.Contains(camPos);
		//	if (!this.isBoxedSelected) {
		//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//		RaycastHit HitInfo;
		//		if (Physics.Raycast(ray, out HitInfo)) {
		//			GameObject obj = HitInfo.collider.gameObject;
		//			Vector3 center = obj.transform.position + new Vector3(0f, 0.5f, 0f);
		//			float distance = Vector3.Distance(HitInfo.point, center);
		//			Selectable selectable = obj.GetComponent<Selectable>();
		//			Debug.Log("Vector distance from " + HitInfo.point.ToString() + " to " + center.ToString() + " is: " + distance.ToString());
		//			if (!obj.name.Equals("Floor") && distance <= 1f && selectable.SelectableID == this.SelectableID) {
		//				this.isBoxedSelected = true;
		//			}
		//		}
		//	}
		//}

		//if (this.isBoxedSelected) {
		//	if (this.selectedColor.Equals(null)) {
		//		Debug.Log("[Selectable] Color is null.");
		//		return;
		//	}
		//	renderer.material.color = this.selectedColor;
		//	if (Input.GetMouseButtonUp(0)) {
		//		this.isSelected = true;
		//		if (!Selectable.selectedObjects.Contains(this)) {
		//			Selectable.selectedObjects.Add(this);
		//		}
		//	}
		//}
		//else {
		//	if (Input.GetMouseButtonUp(0)) {
		//		this.isSelected = false;
		//		while (!this.isSelected && Selectable.selectedObjects.Contains(this)) {
		//			Selectable.selectedObjects.Remove(this);
		//		}
		//	}
		//	renderer.material.color = this.initialColor;
		//}
		#endregion
	}
}