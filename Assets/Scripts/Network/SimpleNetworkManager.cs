using UnityEngine;
using System.Collections;

public class SimpleNetworkManager : MonoBehaviour {
	private HostData[] HostDataArray;
	private string RegisteredGameName = "TEST_THESIS_REAL_TIME_STRATEGY";
	private float RefreshRequestDuration = 3f;
	
	public int totalPlayersCount;

	public void Start() {
		this.totalPlayersCount = 0;
	}

	private void StartServer() {
		Network.InitializeServer(16, 12345, false); //Max connections, port number, use NAT?
		MasterServer.RegisterHost(RegisteredGameName, "Unity RTS Tweaking Network Game", "Testing the implementation of the game.");
	}

	public void OnServerInitialized() {
		Debug.LogError("Simple Network Manager: Server has been initialized.");
		this.totalPlayersCount++;
	}

	private void OnMasterServerEvent(MasterServerEvent msEvent) {
		Debug.LogError("Simple Network Manager: On master server event: " + msEvent.ToString());
	}

	private void OnConnectedToServer() {
		Debug.LogError("Simple Network Manager: Connected to server.");
	}

	private void OnFailedToConnect(NetworkConnectionError Error) {
		Debug.LogError("Simple Network Manager: Could not connect to server. Reason: " + Error.ToString());
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.LogError("Simple Network Manager: Disconnected from server.");
		this.totalPlayersCount--;
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.LogError("Simple Network Manager: Disconnected by player.");
		this.totalPlayersCount--;
	}

	private IEnumerator CR_RefreshHostList() {
		MasterServer.RequestHostList(RegisteredGameName);

		float StartTime = Time.time;
		float EndTime = StartTime + RefreshRequestDuration;
		while (Time.time < EndTime) {
			HostDataArray = MasterServer.PollHostList();
			yield return new WaitForEndOfFrame();
		}

		if (HostDataArray != null && HostDataArray.Length != 0) {
			if (HostDataArray.Length > 1) {
				Debug.Log(HostDataArray.Length + " have been found.");
			}
			else {
				Debug.Log(HostDataArray.Length + " has been found.");
			}
		}
	}

	public void OnGUI() {
		if (Network.isServer) {
			GUILayout.Label("Running as server.");
		}
		else if (Network.isClient) {
			GUILayout.Label("Running as client.");
		}

		if (Network.isClient || Network.isServer) {
			if (GUI.Button(new Rect(50f, 50f, 150f, 30f), "Disconnect")) {
				Network.Disconnect();
				MasterServer.UnregisterHost();
			}
		}

		if (!Network.isClient && !Network.isServer) {
			GUILayout.Label("Doing nothing.");

			if (GUI.Button(new Rect(50f, 50f, 150f, 30f), "Start New Server")) {
				this.StartServer();
			}
			else if (GUI.Button(new Rect(50f, 90f, 150f, 30f), "Refresh Server List")) {
				this.StartCoroutine(CR_RefreshHostList());
			}

			if (HostDataArray != null) {
				for (int i = 0; i < HostDataArray.Length; i++) {
					if (GUI.Button(new Rect(Screen.width / 2f, 65f + (30f * i), 300f, 30f), HostDataArray[i].gameName)) {
						NetworkConnectionError Error = Network.Connect(HostDataArray[i]);
						if (Error != NetworkConnectionError.NoError) {
							Debug.Log("Error: " + Error.ToString());
						}
					}
				}
			}
		}
	}
}
