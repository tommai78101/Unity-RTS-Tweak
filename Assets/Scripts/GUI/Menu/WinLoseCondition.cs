using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinLoseCondition : MonoBehaviour {
	public NetworkView playerNetworkView;
	public bool playerHasLost;
	public static List<NetworkPlayer> Losers;

	public delegate void DG_PlayerLost();
	public event DG_PlayerLost EVENT_PlayerHasLost;

	private void Start() {
		this.playerNetworkView = this.gameObject.GetComponent<NetworkView>();
		if (this.playerNetworkView == null) {
			Debug.LogError(new System.NullReferenceException("WinLoseCondition: Unable to find network view."));
		}
		this.playerHasLost = false;
		Debug.Log("WinLoseCondition: Adding SetPlayerLost() to event delegate.");
		this.EVENT_PlayerHasLost += new WinLoseCondition.DG_PlayerLost(SetPlayerLost);
	}

	private void OnGUI() {
		if (this.playerHasLost) {
			GUI.Label(new Rect((Screen.width - 50) / 2f, (Screen.height - 15) / 2f, 100f, 30f), "YOU LOSE!");
		}
	}

	//[RPC]
	//public void RPC_NotifyWinLose(NetworkViewID loserID) {
	//	NetworkView viewID = NetworkView.Find(loserID);
	//	WinLoseCondition.Losers.Add(viewID.owner);
	//	WinLoseCondition condition = viewID.gameObject.GetComponent<WinLoseCondition>();
	//	if (condition != null) {
	//		condition.playerHasLost = true;
	//	}
	//}

	public void PlayerHasLost() {
		Debug.Log("WinLoseCondition: The player has just lost. Calling PlayerHasLost().");
		//if (UnitManager.Instance.PlayerUnits.Count <= 0 && !this.playerHasLost && Network.connections.Length > 0) {
		//	this.playerNetworkView.RPC("RPC_NotifyWinLose", RPCMode.AllBuffered, this.playerNetworkView.viewID);
		//}
		this.EVENT_PlayerHasLost();
	}

	public void SetPlayerLost() {
		Debug.Log("WinLoseCondition: Calling SetPlayerLost().");
		this.playerHasLost = true;
	}

}
