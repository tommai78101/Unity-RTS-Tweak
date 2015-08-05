using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/*
	Steps to use:

	[Client -> Server] is the [Command] attribute.
	Use [ClientCallback] to call a [Command] method.
	Use [Command] to "sync" or give corrections to an important/useful variable.
	Use [SyncVar] to "sync" the important/useful variable across all clients.
	Use the important/useful variable to update other game object related properties.

*/

public class Test : NetworkBehaviour {
	[SyncVar]
	private NavMeshAgent agent;

	void Start() {
		if (this.isLocalPlayer) {
			Debug.Log("Local Player: I'm " + (this.isServer ? "Server" : "Client") + ".");
		}
		this.agent = this.GetComponent<NavMeshAgent>();
	}

	void Update() {
		if (!this.isLocalPlayer) {
			return;
		}
		GiveTarget();
	}

	[ClientRpc]
	private void RpcSetTarget(Vector3 target) {
		this.agent.SetDestination(target);
	}

	[Command]
	private void CmdMove(Vector3 newPosition) {
		this.agent.SetDestination(newPosition);
		RpcSetTarget(newPosition);
	}

	[ClientCallback]
	private void GiveTarget() {
		if (Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hitInfos = Physics.RaycastAll(ray);
			foreach (RaycastHit hit in hitInfos) {
				GameObject obj = hit.collider.gameObject;
				if (obj.name.Equals("Floor")) {
					CmdMove(hit.point);
					break;
				}
			}
		}
	}
}
