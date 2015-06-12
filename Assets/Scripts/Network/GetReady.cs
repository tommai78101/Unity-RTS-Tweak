using UnityEngine;
using System.Collections;

public class GetReady : MonoBehaviour {
	private bool serverIsReady;
	private bool clientIsReady;
	private bool everythingIsReady;

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isReading) {
			bool serverReadyFlag = false, clientReadyFlag = false;
			stream.Serialize(ref serverReadyFlag);
			stream.Serialize(ref clientReadyFlag);

			if (Network.isClient) {
				this.serverIsReady = serverReadyFlag;
			}
			else if (Network.isServer) {
				this.clientIsReady = clientReadyFlag;
			}

			if (serverReadyFlag && clientReadyFlag) {
				this.everythingIsReady = true;
			}
		}
		else if (stream.isWriting) {
			stream.Serialize(ref this.serverIsReady);
			stream.Serialize(ref this.clientIsReady);
		}
	}

	private void OnPlayerConnected(NetworkPlayer player) {
		this.clientIsReady = true;
		if (this.serverIsReady) {
			this.everythingIsReady = true;
		}
	}

	private void OnConnectedToServer() {
		this.clientIsReady = true;
		if (this.serverIsReady) {
			this.everythingIsReady = true;
		}
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			this.serverIsReady = false;
			this.everythingIsReady = false;
		}
		else if (Network.isClient) {
			this.clientIsReady = false;
			this.everythingIsReady = false;
		}
	}

	private void OnPlayerDisconnected(NetworkPlayer player) {
		this.everythingIsReady = false;
		this.clientIsReady = false;
	}

	private void OnServerInitialized() {
		this.serverIsReady = true;
	}

	private void OnGUI() {
		if (this.everythingIsReady) {
			GUI.contentColor = Color.black;
			GUI.Label(new Rect(150f, 0f, 400f, 30f), "This is now connected and working.");
		}
	}

	private void Start() {
		this.serverIsReady = false;
		this.clientIsReady = false;
		this.everythingIsReady = false;
	}

	public bool IsReady() {
		return this.everythingIsReady;
	}

	[RPC]
	public void RPC_Ready() {
		if (Network.isClient) {
			this.clientIsReady = true;
		}
		else if (Network.isServer) {
			this.serverIsReady = true;
		}
	}
}
