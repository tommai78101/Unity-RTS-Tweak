using UnityEngine;
using System.Collections;

/*
 * TODO: Find a way to get the client to check with the server for current player number count.
 * 
 * Requires:
 * 
 * RPC, Network calls, etc.
 * A clear awaken mind.
 * 
 * 
 * When calling Network.Instantiate() successfully, the caller is the owner of the game object.
 * 
 */


public class Initialize : MonoBehaviour {
	//public GetReady getReadyObject;
	public int playerNumber;
	public int currentHighestNumber;
	public bool serverHasSpawned;
	public GameObject[] playerSpawns;
	public NetworkView playerNetworkView;

	// Use this for initialization
	void Start() {
		this.playerNumber = -1;
		//this.getReadyObject = this.GetComponent<GetReady>();
		//if (this.getReadyObject == null) {
		//	Debug.LogException(new System.NullReferenceException("Get Ready object is null."));
		//}
		this.playerNetworkView = this.GetComponent<NetworkView>();
		if (this.playerNetworkView == null) {
			Debug.LogException(new System.NullReferenceException("Network View object is null."));
		}
		this.serverHasSpawned = false;
	}

	//Doc: Called on the client when you have successfully connected to a server.
	public void OnConnectedToServer() {
		this.playerNumber = 0;
	}

	//public void OnDisconnectedFromMasterServer(NetworkDisconnection info) {
	//	Debug.LogWarning("Initialize: On disconnected from master server.");
	//}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer && Network.connections.Length <= 0) {
			this.serverHasSpawned = false;
		}
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

	//Doc: Called on the server whenever a new player has successfully connected.
	public void OnPlayerConnected(NetworkPlayer player) {
		NetworkView view = this.GetComponent<NetworkView>();
		if (view != null) {
			this.playerNumber = 0;
			this.currentHighestNumber = this.playerNumber;
			view.RPC("RPC_Client_DrawNewNumber", player, this.currentHighestNumber);
		}
	}

	//public void OnPlayerDisconnected(NetworkPlayer player) {
	//	Debug.LogWarning("Initialize: On player disconnected.");
	//}


	//Doc: Called on the server whenever a Network.InitializeServer was invoked and has completed.
	public void OnServerInitialized() {
		Debug.LogWarning("Initialize: On server initialized.");
		this.playerNumber = 0; //The host (server) will always be zero.
		Debug.Log("Player number is now: " + this.playerNumber.ToString());
	}

	//[RPC]
	//public void RPC_Server_Call() {
	//	string type = (Network.isClient ? "(Client)" : "(Server)");
	//	Debug.Log("Client_Call: Sending to server only. Recipient: " + type);
	//	NetworkPlayer[] players = Network.connections;
	//	NetworkView networkView = this.GetComponent<NetworkView>();
	//	this.playerNumber = players.Length;
	//	if (this.playerSpawns != null && this.playerSpawns.Length > 0) {
	//		for (int i = 0; i <= this.playerNumber; i++) {
	//			if (networkView != null && networkView.isMine) {
	//				GameObject gameObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.playerSpawns[i].transform.position, Quaternion.identity, i);
	//				if (gameObject != null) {
	//					gameObject.name = gameObject + " " + this.playerNumber;
	//				}
	//				else {
	//					Debug.LogError("Failed to instantiate player.");
	//				}
	//			}
	//		}
	//	}
	//	//this.StartCoroutine(CR_RPC_Server_Call_Wait(players));
	//}

	//private IEnumerator CR_RPC_Server_Call_Wait(NetworkPlayer[] players) {
	//}

	[RPC]
	public void RPC_Client_DrawNewNumber(int serverNumber) {
		if (Network.isClient) {
			this.playerNumber = serverNumber + 1;
			Debug.Log("serverNumber: " + serverNumber.ToString() + ". playerNumber: " + this.playerNumber.ToString());
			this.playerNetworkView.RPC("RPC_Server_Validate", RPCMode.Server, this.playerNumber, Network.player);
		}
	}

	[RPC]
	public void RPC_Client_Spawn(int newNumber) {
		if (Network.isClient) {
			this.playerNumber = newNumber;
			Debug.Log("newNumber: " + newNumber.ToString() + ". playerNumber: " + this.playerNumber.ToString());
			GameObject gameObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.playerSpawns[this.playerNumber].transform.position, Quaternion.identity, 0);
			if (gameObject != null) {
				//Original initialized untis will have parentheses around the remote label (client or server).
				gameObject.name = (Network.isClient ? "(client)" : "(server)") + " " + System.Guid.NewGuid().ToString();
				this.playerNetworkView.RPC("RPC_Server_Spawn", RPCMode.Server, null);
			}
			else {
				Debug.LogError("Failed to instantiate player.");
			}
		}
	}

	[RPC]
	public void RPC_Server_Validate(int number, NetworkPlayer player) {
		if (Network.isServer) {
			Debug.Log("number: " + number.ToString() + ". server_currentHighestNumber: " + this.currentHighestNumber.ToString());
			if (this.currentHighestNumber < number) {
				this.playerNetworkView.RPC("RPC_Client_Spawn", player, number);
				this.currentHighestNumber = number;
				Debug.Log("After client spawning, number: " + number.ToString() + ". server_currentHighestNumber: " + this.currentHighestNumber.ToString());
			}
			else {
				this.playerNetworkView.RPC("RPC_Client_DrawNewNumber", player, this.currentHighestNumber);
			}
		}
	}

	[RPC]
	public void RPC_Server_Spawn() {
		if (Network.isServer) {
			if (!this.serverHasSpawned) {
				GameObject gameObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.playerSpawns[this.playerNumber].transform.position, Quaternion.identity, 0);
				if (gameObject != null) {
					gameObject.name = gameObject + " " + this.playerNumber + " " + (Network.isClient ? "(Client)" : "(Server)");
				}
				else {
					Debug.LogError("Failed to instantiate player.");
				}
				this.serverHasSpawned = true;
			}
		}
	}
}
