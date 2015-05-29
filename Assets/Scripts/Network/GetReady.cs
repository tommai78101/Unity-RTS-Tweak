using UnityEngine;
using System.Collections;

public class GetReady : MonoBehaviour {
	private bool serverIsReady;
	private bool clientIsReady;
	private bool everythingIsReady;

	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
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

	public void OnPlayerConnected(NetworkPlayer player) {
		this.clientIsReady = true;
		if (this.serverIsReady) {
			this.everythingIsReady = true;
		}
	}

	public void OnConnectedToServer() {
		this.clientIsReady = true;
		if (this.serverIsReady) {
			this.everythingIsReady = true;
		}
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			this.serverIsReady = false;
			this.everythingIsReady = false;
		}
		else if (Network.isClient) {
			this.clientIsReady = false;
			this.everythingIsReady = false;
		}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		this.everythingIsReady = false;
		this.clientIsReady = false;
	}

	public void OnServerInitialized() {
		this.serverIsReady = true;
	}

	public void OnGUI() {
		if (this.everythingIsReady) {
			GUI.contentColor = Color.black;
			GUI.Label(new Rect(150f, 0f, 400f, 30f), "This is now connected and working.");
		}
	}

	public void Start() {
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
