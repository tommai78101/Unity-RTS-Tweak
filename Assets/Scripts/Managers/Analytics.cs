using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Analytics : MonoBehaviour {
	public static Analytics Instance;

	public float sessionTimer;
	public bool sessionTimerTickFlag;
	public List<float> sessionTimerData;
	public bool serverSessionReadyFlag;

	private NetworkView analyticNetworkView;

	public void Start(){
		Analytics.Instance = this;
		this.sessionTimer = 0f;
		this.sessionTimerTickFlag = false;
		this.sessionTimerData = new List<float>();
		this.analyticNetworkView = this.GetComponent<NetworkView>();
	}

	public void OnPlayerConnected(NetworkPlayer player) {
		Debug.LogWarning("Player has connected. Timer analytic stuff set to start.");
		if (this.analyticNetworkView != null && this.analyticNetworkView.isMine) {
			this.analyticNetworkView.RPC("RPC_StartTimer", RPCMode.AllBuffered);
		}
	}

	public void Update() {
		if (this.sessionTimerTickFlag) {
			this.sessionTimer += Time.deltaTime;
		}
	}

	public void StopTimer() {
		Debug.LogWarning("Session Timer has stopped.");
		this.sessionTimerTickFlag = false;
		this.sessionTimerData.Add(this.sessionTimer);
	}

	public void StartTimer() {
		Debug.LogWarning("Session Timer has started.");
		this.sessionTimerTickFlag = true;
	}

	[RPC]
	public void RPC_StartTimer() {
		this.StartTimer();
	}
}
