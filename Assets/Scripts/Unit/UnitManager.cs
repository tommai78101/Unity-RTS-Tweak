using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnitManager : MonoBehaviour {
	public static UnitManager Instance;
	public static readonly int PG_Player = 1;
	public static readonly int PG_Enemy = 2;
	public static readonly int MAX_UNIT_LIMIT = 5;

	public List<GameObject> AllUnits = new List<GameObject>();
	public List<GameObject> PlayerUnits = new List<GameObject>();

	private NetworkView playerNetworkView;

	public void Awake() {
		UnitManager.Instance = this;
		this.playerNetworkView = this.GetComponent<NetworkView>();
		if (this.playerNetworkView == null) {
			Debug.LogException(new System.NullReferenceException("Network view for Unit Manager is null. Something's wrong."));
		}
	}

	public void OnGUI() {
		if (Input.GetKeyDown(KeyCode.M)) {
			Debug.Log("Player unit count: " + this.PlayerUnits.Count);
			Debug.Log("All unit count: " + this.AllUnits.Count);
		}
	}

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.LogWarning("Unit Manager: On disconnected from server.");
		NetworkPlayer[] players = Network.connections;
		for (int i = 0; i < this.AllUnits.Count; i++) {
			if (players.Length > 0) {
				if (!this.AllUnits[i].activeSelf && this.playerNetworkView.isMine) {
					Network.Destroy(this.AllUnits[i]);
				}
			}
			else {
				Object.Destroy(this.AllUnits[i]);
			}
		}
		if (this.AllUnits.Count > 0) {
			this.AllUnits.Clear();
		}
	}

	public void OnConnectedToServer() {
		if (UnitManager.Instance == null) {
			UnitManager.Instance = this;
		}
	}

	public void OnPlayerConnected(NetworkPlayer player) {
		if (UnitManager.Instance == null) {
			UnitManager.Instance = this;
		}
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		if (this.AllUnits.Count > 0) {
			this.AllUnits.Clear();
		}
	}

	public static void DestroyAllUnits(NetworkPlayer[] connections) {
		Debug.LogError("Simple Network Manager: Disconnected by player.");
		foreach (GameObject g in UnitManager.Instance.AllUnits) {
			g.SetActive(false);
			if (!g.activeSelf && UnitManager.Instance.playerNetworkView.isMine) {
				if (connections.Length > 0) {
					Network.Destroy(g);
				}
				else {
					Object.Destroy(g);
				}
			}
		}
	}
}