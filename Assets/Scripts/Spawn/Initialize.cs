using UnityEngine;
using System.Collections;

public class Initialize : MonoBehaviour {
	public GetReady getReadyObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnConnectedToServer() {

	}

	public void OnDisconnectedFromMasterServer(NetworkDisconnection info) {

	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {

	}

	public void OnFailedToConnect(NetworkConnectionError error) {

	}

	public void OnFailedToConnectToMasterServer(NetworkConnectionError error) {

	}

	public void OnMasterServerEvent(MasterServerEvent msEvent) {

	}

	public void OnPlayerConnected(NetworkPlayer player) {
		if (this.getReadyObject.IsReady()) {
			//Do...
		}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		
	}

	public void OnServerInitialized() {
		
	}


}
