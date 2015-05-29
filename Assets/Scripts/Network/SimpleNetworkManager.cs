using UnityEngine;
using System.Collections;

public class SimpleNetworkManager : MonoBehaviour {
	private HostData[] HostDataArray;
	private string RegisteredGameName = "TEST_THESIS_REAL_TIME_STRATEGY";
	private float RefreshRequestDuration = 3f;

	private void Start() {
		Debug.Log("Starting the simple network manager.");
	}

	private void StartServer() {
		Network.InitializeServer(16, 12345, false); //Max connections, port number, use NAT?
		MasterServer.RegisterHost(RegisteredGameName, "Unity RTS Tweaking Network Game", "Testing the implementation of the game.");
	}

	public void OnServerInitialized() {
		Debug.Log("Server has been initialized.");
	}

	private void OnMasterServerEvent(MasterServerEvent MasterServerEvent) {
		if (MasterServerEvent == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("Master server registration successful.");
		}
	}

	private void OnConnectedToServer() {
		Debug.Log("Connected to server.");
	}

	private void OnFailedToConnect(NetworkConnectionError Error) {
		Debug.LogError("Could not connect to server. Reason: " + Error.ToString());
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Disconnected from server.");
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Disconnected by player.");
	}

	private IEnumerator CR_RefreshHostList() {
		Debug.Log("Refreshing host list...");
		MasterServer.RequestHostList(RegisteredGameName);

		float StartTime = Time.time;
		float EndTime = StartTime + RefreshRequestDuration;
		while (Time.time < EndTime) {
			HostDataArray = MasterServer.PollHostList();
			yield return new WaitForEndOfFrame();
		}

		if (HostDataArray == null || HostDataArray.Length == 0) {
			Debug.Log("No active servers available.");
		}
		else {
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
				Debug.Log("Disconnecting.");
				Network.Disconnect();
				MasterServer.UnregisterHost();
			}
		}

		if (!Network.isClient && !Network.isServer) {
			GUILayout.Label("Doing nothing.");

			if (GUI.Button(new Rect(50f, 50f, 150f, 30f), "Start New Server")) {
				Debug.Log("Starting new server.");
				this.StartServer();
			}
			else if (GUI.Button(new Rect(50f, 90f, 150f, 30f), "Refresh Server List")) {
				Debug.Log("Refreshing server list. Nothing happened.");
				this.StartCoroutine(CR_RefreshHostList());
			}

			if (HostDataArray != null) {
				for (int i = 0; i < HostDataArray.Length; i++) {
					if (GUI.Button(new Rect(Screen.width / 2f, 65f + (30f * i), 300f, 30f), HostDataArray[i].gameName)) {
						NetworkConnectionError Error = Network.Connect(HostDataArray[i]);
						if (Error == NetworkConnectionError.NoError) {
							Debug.Log("No error in sight!");
						}
						else {
							Debug.Log("Error: " + Error.ToString());
						}
					}
				}
			}
		}
	}
}
