using UnityEngine;
using System.Collections;

public class Initialize : MonoBehaviour {
	public GetReady getReadyObject;
	public int playerNumber;
	public GameObject[] playerSpawns;

	// Use this for initialization
	void Start() {
		this.playerNumber = -1;
		GetReady getReady = this.GetComponent<GetReady>();
		if (getReady != null) {
			this.getReadyObject = getReady;
		}
	}

	public void OnConnectedToServer() {
		//string type = (Network.isClient ? "(Client)" : "(Server)");
		//Debug.LogWarning("Initialize: On connected to server as client as " + type);
		NetworkView networkView = this.GetComponent<NetworkView>();
		if (networkView != null) {
			networkView.RPC("RPC_Server_Call", RPCMode.Server, null);
		}
		//else {
		//	Debug.LogWarning("Initialize: Couldn't find network view. " + (Network.isClient ? "(Client)" : "(Server)"));
		//}
	}

	//public void OnDisconnectedFromMasterServer(NetworkDisconnection info) {
	//	Debug.LogWarning("Initialize: On disconnected from master server.");
	//}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		//string type = (Network.isClient ? "(Client)" : "(Server)");
		//Debug.LogWarning("Initialize: On disconnected to server as " + type);
		this.playerNumber = -1;
		Debug.Log("Player number is now: " + this.playerNumber.ToString());
	}

	//public void OnFailedToConnect(NetworkConnectionError error) {
	//	Debug.LogWarning("Initialize: On failed to connect.");
	//}

	//public void OnFailedToConnectToMasterServer(NetworkConnectionError error) {
	//	Debug.LogWarning("Initialize: On failed to connect to master server.");
	//}

	//public void OnMasterServerEvent(MasterServerEvent msEvent) {
	//	Debug.LogWarning("Initialize: On master server event: " + msEvent.ToString());
	//}

	public void OnPlayerConnected(NetworkPlayer player) {
		//string type = (Network.isClient ? "(Client)" : "(Server)");
		//Debug.LogWarning("Initialize: On player connected as server as " + type);
		NetworkView networkView = this.GetComponent<NetworkView>();
		if (networkView != null) {
			networkView.RPC("RPC_Client_DrawNewNumber", RPCMode.OthersBuffered, new object[]{ this.playerNumber });
			networkView.RPC("RPC_Client_Call", RPCMode.OthersBuffered, null);
		}
		//else {
		//	Debug.LogWarning("Initialize: Couldn't find network view. " + (Network.isClient ? "(Client)" : "(Server)"));
		//}
	}

	//public void OnPlayerDisconnected(NetworkPlayer player) {
	//	Debug.LogWarning("Initialize: On player disconnected.");
	//}

	public void OnServerInitialized() {
		//Debug.LogWarning("Initialize: On server initialized.");
		this.playerNumber = 1;
		Debug.Log("Player number is now: " + this.playerNumber.ToString());
	}

	[RPC]
	public void RPC_Server_Call() {
		//string type = (Network.isClient ? "(Client)" : "(Server)");
		//Debug.Log("Server_Call: Sending to server only. Recipient: " + type);
		if (this.playerSpawns != null && this.playerSpawns.Length > 0 && this.playerSpawns.Length > this.playerNumber-1) {
			GameObject gameObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.playerSpawns[this.playerNumber-1].transform.position, Quaternion.identity, 0);
			if (gameObject != null) {
				gameObject.name = gameObject + " " + this.playerNumber;
			}
		}
	}

	[RPC]
	public void RPC_Client_Call() {
		//string type = (Network.isClient ? "(Client)" : "(Server)");
		//Debug.Log("Client_Call: Sending to client only. Recipient: " + type);
		if (this.playerSpawns != null && this.playerSpawns.Length > 0 && this.playerSpawns.Length > this.playerNumber-1) {
			GameObject gameObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.playerSpawns[this.playerNumber-1].transform.position, Quaternion.identity, 0);
			if (gameObject != null) {
				gameObject.name = gameObject + " " + this.playerNumber;
			}
		}
	}

	[RPC]
	public void RPC_Client_DrawNewNumber(int serverPlayerNumber) {
		string type = (Network.isClient ? "(Client)" : "(Server)");
		//Debug.Log("Client_DrawNewNumber: Sending to client only. Recipient: " + type);
		this.playerNumber = serverPlayerNumber + 1;
		Debug.Log("Player number is now " + this.playerNumber.ToString() + ". Server player number is: " + serverPlayerNumber.ToString() + ". Recipient: " + type);
	}
}
