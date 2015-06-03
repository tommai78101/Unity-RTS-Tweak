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

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.LogWarning("Unit Manager: On disconnected from server.");
		foreach (GameObject g in this.AllUnits) {
			Object.Destroy(g);
		}
		if (this.AllUnits.Count > 0) {
			this.AllUnits.Clear();
		}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.LogError("Simple Network Manager: Disconnected by player.");
		foreach (GameObject g in this.AllUnits) {
			Object.Destroy(g);
		}
		if (this.AllUnits.Count > 0) {
			this.AllUnits.Clear();
		}
	}
}