using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestPrefab : NetworkBehaviour {
	public GameObject something;

	public override void OnStartLocalPlayer() {
		base.OnStartLocalPlayer();

	}

	void Update() {
		if (this.isLocalPlayer || this.hasAuthority) {
			Debug.Log("Is local player.");
			if (Input.GetMouseButton(0)) {
				if (this.isServer) {
					RpcCall();
				}
				else {
					CmdCall();
				}
			}
		}
	}

	[Command]
	public void CmdCall() {
		RpcCall();
	}

	[ClientRpc]
	public void RpcCall() {
		Debug.Log("RPC Called.");
	}
}