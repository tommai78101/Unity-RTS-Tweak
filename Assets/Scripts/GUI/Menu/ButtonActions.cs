using UnityEngine;
using System.Collections;

public class ButtonActions : MonoBehaviour {
	public static string RegisteredHostName {
		get {
			return "UNITY_MENU_GUI_BUTTONS";
		}
	}
	private int serverIndex;

	//public HostData[] HostDataArray;
	public int totalPlayersCount;
	public GameObject listPrefab;

	//Button actions

	public void HostServer() {
		Debug.Log("Hosting.");
		Network.InitializeServer(16, 12345, false);
		MasterServer.RegisterHost(ButtonActions.RegisteredHostName, "Unity RTS Prototype");
		Debug.Log("Immediately loading host game.");
		Application.LoadLevel("test_nav");
	}

	public void DisconnectFromServer() {
		Debug.Log("Disconnecting.");
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}

	public void Quit() {
		Debug.Log("Quitting.");
		Application.Quit();
	}

	public void ConnectToLocalHost() {
		NetworkConnectionError error = Network.Connect("localhost", 12345);
		if (error != NetworkConnectionError.NoError) {
			Debug.Log("Server is probably not hosted, or the connection is blocked.");
		}
		else {
			Application.LoadLevel("test_nav");
		}
	}

	//MonoBehaviours

	public void Start() {
		this.totalPlayersCount = 0;
	}

	public void OnServerInitialized() {
		this.totalPlayersCount++;
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		this.totalPlayersCount--;
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		this.totalPlayersCount--;
		if (this.totalPlayersCount <= 1) {
			Debug.Log("All other players are gone. Host is auto-kicked.");
			Application.LoadLevel("menu");
		}
	}

	public void OnPlayerConnected(NetworkPlayer player) {
		if (this.totalPlayersCount <= 1) {
			Debug.Log("At least 1 player has connected. Loading level.");
			Application.LoadLevel("test_nav");
		}
	}

	public void OnGUI() {
		if (Network.isServer) {
			GUILayout.Label("Running as server.");
		}
		else if (Network.isClient) {
			GUILayout.Label("Running as client.");
		}

		//if (Network.isClient || Network.isServer) {
		//	if (GUI.Button(new Rect(50f, 50f, 150f, 30f), "Disconnect")) {

		//	}
		//}

		if (!Network.isClient && !Network.isServer) {
			GUILayout.Label("Doing nothing.");

			//if (GUI.Button(new Rect(50f, 50f, 150f, 30f), "Start New Server")) {
			//	this.StartServer();
			//}
			//else if (GUI.Button(new Rect(50f, 90f, 150f, 30f), "Refresh Server List")) {

			//}

			//if (HostDataArray != null) {
			//	for (int i = 0; i < HostDataArray.Length; i++) {
			//		if (GUI.Button(new Rect(Screen.width / 2f, 65f + (30f * i), 300f, 30f), HostDataArray[i].gameName)) {
			//			NetworkConnectionError Error = Network.Connect(HostDataArray[i]);
			//			if (Error != NetworkConnectionError.NoError) {
			//				Debug.Log("Error: " + Error.ToString());
			//			}
			//		}
			//	}
			//}
		}
	}

	//Co routines

	//private IEnumerator CR_RefreshHostList() {
	//	if (this.HostDataArray != null && this.HostDataArray.Length > 0) {
	//		System.Array.Clear(this.HostDataArray, 0, this.HostDataArray.Length);
	//	}
	//	MasterServer.RequestHostList(ButtonActions.RegisteredHostName);

	//	float StartTime = Time.time;
	//	float EndTime = StartTime + RefreshRequestDuration;
	//	while (Time.time < EndTime) {
	//		HostDataArray = MasterServer.PollHostList();
	//		yield return new WaitForEndOfFrame();
	//	}

	//	if (HostDataArray != null && HostDataArray.Length != 0) {
	//		if (HostDataArray.Length > 1) {
	//			Debug.Log(HostDataArray.Length + " have been found.");
	//		}
	//		else {
	//			Debug.Log(HostDataArray.Length + " has been found.");
	//		}
	//		if (this.listPrefab != null) {
	//			Object.Instantiate<GameObject>(this.listPrefab);
	//		}
	//	}
	//}
}
