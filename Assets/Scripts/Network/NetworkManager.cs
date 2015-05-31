using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	//public GameObject[] SpawnLocations;
	//public int SpawnLocationNumber;

	private HostData[] HostDataArray;
	private string RegisteredGameName = "TEST_THESIS_REAL_TIME_STRATEGY";
	private float RefreshRequestDuration = 3f;
	//private int SpawnCounter = 0;

	//[RPC]
	//public void CheckSpawnedUnitsCount() {
	//	GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
	//	foreach (GameObject g in gameObjects) {
	//		if (!UnitManager.PlayerUnits.Contains(g)) {
	//			UnitManager.PlayerUnits.Add(g);
	//		}
	//	}
	//}

	//[RPC]
	//public void RPC_SpawnPlayer() {
	//	GameObject SpawnLocation = this.SpawnLocations[this.SpawnLocationNumber];
	//	Vector3 spawnLocation = SpawnLocation.transform.position;
	//	bool tryAgain = true;
	//	while (tryAgain) {
	//		tryAgain = false;
	//		Collider[] colliders = Physics.OverlapSphere(spawnLocation, 0.6f);
	//		Gizmos.DrawWireSphere(spawnLocation, 0.6f);
	//		foreach (Collider c in colliders) {
	//			if (c.gameObject.name.Equals("Floor")) {
	//				continue;
	//			}
	//			if (c.gameObject.name.StartsWith("Player")) {
	//				tryAgain = true;
	//				break;
	//			}
	//		}
	//		if (tryAgain) {
	//			spawnLocation.x += 1.05f;
	//		}
	//	}

	//	GameObject spawnedObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), spawnLocation, Quaternion.identity, UnitManager.PG_Player);
	//	spawnedObject.name = spawnedObject.name + " " + string.Format("{0}", this.SpawnCounter);
	//	spawnedObject.tag = "Player";
	//	if (!UnitManager.PlayerUnits.Contains(spawnedObject)) {
	//		UnitManager.PlayerUnits.Add(spawnedObject);
	//		this.SpawnCounter++;
	//	}

	//	Debug.Log("Unit has spawned. Spawn Counter: " + this.SpawnCounter.ToString());
	//}

	private void Start() {
		//if (this.SpawnLocations.Length <= 0) {
		//	Debug.LogError("Please initialize the spawn locations array with starting location game objects.");
		//}
	}

	private void StartServer() {
		Network.InitializeServer(16, 12345, false); //Max connections, port number, use NAT?
		MasterServer.RegisterHost(RegisteredGameName, "Unity RTS Tweaking Network Game", "Testing the implementation of the game.");

		//SpawnPlayer();
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		//stream.Serialize(ref this.SpawnCounter);
		//stream.Serialize(ref this.SpawnLocationNumber);
	}

	private void OnMasterServerEvent(MasterServerEvent msEvent) {
		Debug.Log("Master server event: " + msEvent.ToString());
	}

	private void OnConnectedToServer() {
		Debug.Log("Connected to server, now spawning player...");
		//SpawnPlayer();
	}

	private void OnFailedToConnect(NetworkConnectionError Error) {
		Debug.LogError("Could not connect to server. Reason: " + Error.ToString());
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Disconnected from server, now initiating destruction of game objects. Count: " + UnitManager.instance.PlayerUnits.Count);
		//while (UnitManager.PlayerUnits.Count > 0){
		//	Debug.Log("Destroying objects: " + UnitManager.PlayerUnits[0].ToString());
		//	Destroy(UnitManager.PlayerUnits[0]);
		//	UnitManager.PlayerUnits.RemoveAt(0);
		//}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Disconnected by player.");
	}

	//private void SpawnPlayer() {
	//	#region UNWANTED_COMMENTS
	//	//GameObject SpawnLocation = this.SpawnLocations[this.SpawnLocationNumber];
	//	//this.PlayerNumber = this.SpawnLocationNumber;
	//	//this.SpawnLocationNumber++;

	//	//Debug.Log("Spawning new player.");
	//	//Object Result = Network.Instantiate(Resources.Load("Prefabs/Player"), SpawnLocation.transform.position, Quaternion.identity, this.PlayerNumber);
	//	//Debug.LogWarning("Result from Instantiate: " + Result.ToString() + ", group number: " + this.PlayerNumber);

	//	//Debug.Log("Added game object Result: " + Result.ToString());
	//	//UnitManager.PlayerUnits.Add((GameObject) Result);
	//	//Debug.Log("Unit manager player units count: " + UnitManager.PlayerUnits.Count);

	//	//NetworkView NetworkView = this.GetComponent<NetworkView>();
	//	//if (NetworkView != null) {
	//	//	NetworkView.RPC("SpawnLocationNumberUpdateAndNotify", RPCMode.OthersBuffered, new object[] { this.SpawnLocationNumber });
	//	//}
		

	//	//GameObject SpawnLocation = this.SpawnLocations[this.SpawnLocationNumber];
	//	//Vector3 spawnLocation = SpawnLocation.transform.position;
	//	//bool tryAgain = true;
	//	//while (tryAgain) {
	//	//	tryAgain = false;
	//	//	Collider[] colliders = Physics.OverlapSphere(spawnLocation, 0.6f);
	//	//	foreach (Collider c in colliders) {
	//	//		if (c.gameObject.name.Equals("Floor")) {
	//	//			continue;
	//	//		}
	//	//		if (c.gameObject.tag.Equals("TemporaryUnit") || c.gameObject.tag.Equals("Player") || c.gameObject.name.Equals("EntityBody")) {
	//	//			tryAgain = true;
	//	//			break;
	//	//		}
	//	//	}
	//	//	if (tryAgain) {
	//	//		spawnLocation.x += 1.05f;
	//	//	}
	//	//}

	//	//GameObject spawnedObject = (GameObject)Network.Instantiate(Resources.Load("Prefabs/Player"), spawnLocation, Quaternion.identity, UnitManager.PG_Player);
	//	//spawnedObject.name = spawnedObject.name + " " + string.Format("{0}", this.SpawnCounter);
	//	//spawnedObject.tag = "Player";
	//	//UnitManager.PlayerUnits.Add(spawnedObject);
	//	//this.SpawnCounter++;
	//	#endregion

	//	NetworkView NetworkView = this.GetComponent<NetworkView>();
	//	if (NetworkView != null) {
	//		NetworkView.RPC("RPC_SpawnPlayer", RPCMode.AllBuffered, null);
	//		NetworkView.RPC("CheckSpawnedUnitsCount", RPCMode.AllBuffered, null);
	//	}
	//}

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
				StartServer();
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
