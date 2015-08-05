using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class InputManager : NetworkBehaviour {
	public enum OrderType {
		ATTACK,
		OTHER
	}

	public List<GameObject> selectedObjects;
	public bool waitingForTarget;
	public OrderType orderType;

	private void Start() {
		this.selectedObjects = new List<GameObject>();
		this.waitingForTarget = false;
	}

	private void Update() {
		TEST_input();
		//InputEvent();
		//if (this.selectedObjects.Count > 0) {
		//	if (this.isLocalPlayer) {
		//		SetSelectionColor(this.selectedObjects, Color.green);
		//	}
		//	else {
		//		SetSelectionColor(this.selectedObjects, Color.magenta);
		//	}
		//}
	}




	private void TEST_input() {
		if (!this.isLocalPlayer) {
			return;
		}
		if (Input.GetMouseButtonDown(1)) {
			Debug.Log("Triggered!");
			CmdMessageToServer();
		}
	}


	[Command]
	private void CmdMessageToServer() {
		Debug.Log("Sending a message to the server.");
	}














	[ClientCallback]
	private void InputEvent() {
		if (this.waitingForTarget) {
			if (Input.GetMouseButtonDown(1)) {
				switch (this.orderType) {
					case OrderType.ATTACK:
						SendAttackOrder(Input.mousePosition);
						break;
				}
			}
		}
		else {
			if (Input.GetMouseButtonDown(0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo)) {
					CheckHitObject(hitInfo);
				}
			}
			else if (Input.GetMouseButtonDown(1)) {
				MoveToTarget();
			}
			else if (Input.GetKeyDown(KeyCode.S)) {
				//Split
			}
			else if (Input.GetKeyDown(KeyCode.D)) {
			}
			if (Input.GetKeyDown(KeyCode.A)) {
				this.waitingForTarget = true;
			}
		}
	}

	//-------------------------------------------------------------------------------------------------

	private void CheckHitObject(RaycastHit hitInfo) {
		GameObject unit = hitInfo.collider.gameObject;
		//CubeUnit unit = obj.GetComponent<CubeUnit>();
		if (unit != null) {
			if (unit.name.Equals("Floor") || unit.tag.Equals("Spawn")) {
				SetSelectionColor(this.selectedObjects, Color.white);
				this.selectedObjects.Clear();
				return;
			}
			else if (!this.selectedObjects.Contains(unit)) {
				this.selectedObjects.Add(unit);
			}
		}
	}

	private void SetSelectionColor(List<GameObject> list, Color color) {
		foreach (GameObject obj in list) {
			if (obj != null) {
				Renderer renderer = obj.GetComponent<Renderer>();
				renderer.material.color = color;
			}
		}
	}

	private void MoveToTarget() {
		foreach (GameObject obj in this.selectedObjects) {
			if (obj != null) {
				NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
				if (agent != null) {
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit[] hitInfos = Physics.RaycastAll(ray);
					foreach (RaycastHit hit in hitInfos) {
						GameObject floor = hit.collider.gameObject;
						if (floor.name.Equals("Floor")) {
							SetDestination(hit.point);
							break;
						}
					}
				}
			}
		}
	}

	private void SendAttackOrder(Vector3 mousePosition) {
		Ray ray = Camera.main.ScreenPointToRay(mousePosition);
		RaycastHit[] hitInfos = Physics.RaycastAll(ray);
		foreach (RaycastHit hit in hitInfos) {
			GameObject obj = hit.collider.gameObject;
			if (obj.name.Equals("Floor")) {
				SetDestination(hit.point);
				break;
			}
		}
	}

	[Command]
	public void CmdSetDestination(GameObject obj, Vector3 target) {
		NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
		agent.SetDestination(target);
	}

	[ClientCallback]
	public void SetDestination(Vector3 target) {
		foreach (GameObject selected in this.selectedObjects) {
			CmdSetDestination(selected, target);
		}
	}
}
