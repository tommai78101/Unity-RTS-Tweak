using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestPrefab : NetworkBehaviour {
	void Update () {
		if (this.isServer) {
			ServerTest();
		}
		else {
			ClientTest();
		}
	}

	public void Call() {
		Debug.Log("Call");
	}

	[Command]
	public void CmdCall() {
		Debug.Log("CMD Call");
	}

	[ClientRpc]
	public void RpcCall() {
		Debug.Log("RPC Call");
		CmdCall();
	}

	public void ServerTest() {
		RpcCall();
	}

	public void ClientTest() {
		Call();
	}
}
