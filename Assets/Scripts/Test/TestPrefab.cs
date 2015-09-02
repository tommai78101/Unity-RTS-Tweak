using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestPrefab : NetworkBehaviour {
	public GameObject something;

	void Start() {
		NetworkServer.Spawn(something);
		ClientScene.RegisterPrefab(something);
	}

	void Update() {
		if (this.isLocalPlayer) {
			if (Input.GetMouseButton(0)) {
				CmdInputs();
			}
		}
	}

	[Command]
	public void CmdInputs() {
		Rpc_ChangeColor();
	}

	[ClientRpc]
	public void Rpc_ChangeColor() {
		Renderer renderer = something.GetComponent<Renderer>();
		if (renderer != null) {
			if (renderer.material.color == Color.red) {
				renderer.material.color = Color.white;
			}
			else {
				renderer.material.color = Color.red;
			}
		}
	}
}