using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnitManager : MonoBehaviour {
	public static UnitManager instance;

	public static readonly int PG_Player = 1;
	public static readonly int PG_Enemy = 2;

	public List<GameObject> PlayerUnits = new List<GameObject>();

	public void Awake() {
		UnitManager.instance = this;
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.LogWarning("Unit Manager: On disconnected from server.");
		foreach (GameObject g in this.PlayerUnits) {
			Object.Destroy(g);
		}
		if (this.PlayerUnits.Count > 0) {
			this.PlayerUnits.Clear();
		}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.LogError("Simple Network Manager: Disconnected by player.");
		foreach (GameObject g in this.PlayerUnits) {
			Object.Destroy(g);
		}
		if (this.PlayerUnits.Count > 0) {
			this.PlayerUnits.Clear();
		}
	}
}