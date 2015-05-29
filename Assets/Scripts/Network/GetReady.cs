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
		Debug.LogWarning("On player connected.");
		this.clientIsReady = true;
		if (this.serverIsReady) {
			this.everythingIsReady = true;
		}
	}

	public void OnConnectedToServer() {
		Debug.LogWarning("On connected to server.");
		this.clientIsReady = true;
		if (this.serverIsReady) {
			this.everythingIsReady = true;
		}
	}

	public void OnDisconnectedFromMasterServer(NetworkDisconnection info) {
		Debug.LogWarning("On disconnected from master server.");
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer) {
			Debug.LogWarning("On disconnecting from server as server.");
			this.serverIsReady = false;
			this.everythingIsReady = false;
		}
		else if (Network.isClient) {
			Debug.LogWarning("On disconnecting from server as client.");
			this.clientIsReady = false;
			this.everythingIsReady = false;
		}
	}

	public void OnFailedToConnect(NetworkConnectionError error) {
		Debug.LogWarning("On failed to connect.");
	}

	public void OnFailedToConnectToMasterServer(NetworkConnectionError error) {
		Debug.LogWarning("On failed to connect to master server.");
	}

	public void OnMasterServerEvent(MasterServerEvent msEvent) {
		Debug.LogWarning("On master server event. Event: " + msEvent.ToString());
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.LogWarning("On player disconnected.");
		this.everythingIsReady = false;
		this.clientIsReady = false;
	}

	public void OnServerInitialized() {
		Debug.LogWarning("On server initialized.");
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
