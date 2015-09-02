using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestPrefab : NetworkBehaviour {
	void Update () {
		if (!this.isServer) {
			Debug.Log("Client test.");
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

	public void MiddleRpcCall() {
		Debug.Log("Middle -> RPC");
		RpcCall();
	}

	public void MiddleCmdCall() {
		Debug.Log("Middle -> Cmd");
		CmdCall();
	}

	public void ServerTest() {
		Debug.Log("Calling on server test.");
		RpcCall();
	}

	public void ClientTest() {
		Debug.Log("Calling on client test.");
		Call();
	}
}
