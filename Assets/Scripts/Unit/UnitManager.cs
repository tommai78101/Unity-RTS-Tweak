using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnitManager : MonoBehaviour {
	public static UnitManager instance;

	public static readonly int PG_Player = 1;
	public static readonly int PG_Enemy = 2;

	public List<GameObject> AllUnits = new List<GameObject>();
	public List<GameObject> PlayerUnits = new List<GameObject>();
	//public List<Pair> SpawnedUnits = new List<Pair>();


	public void Awake() {
		UnitManager.instance = this;
	}

	public void OnGUI() {
		if (Input.GetKeyDown(KeyCode.M)) {
			Debug.Log("Player unit count: " + this.PlayerUnits.Count);
			Debug.Log("All unit count: " + this.AllUnits.Count);
		}
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.LogWarning("Unit Manager: On disconnected from server.");
		for (int i = 0; i < this.AllUnits.Count; i++) {
			Network.Destroy(this.AllUnits[i]);
		}
		if (this.AllUnits.Count > 0) {
			this.AllUnits.Clear();
		}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.LogError("Simple Network Manager: Disconnected by player.");
		foreach (GameObject g in this.AllUnits) {
			Network.Destroy(g);
		}
		if (this.AllUnits.Count > 0) {
			this.AllUnits.Clear();
		}
	}
}