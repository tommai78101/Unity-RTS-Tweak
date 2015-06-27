using UnityEngine;
using System.Collections;

public class Debugging : MonoBehaviour {
	public static bool debugEnabled = true;

	private void Awake() {
		Debugging.debugEnabled = true;
		Network.InitializeServer(16, 12345, false);
		MasterServer.RegisterHost("Debug", "Debug");
	}

	private void Start() {
		NetworkView view = this.GetComponent<NetworkView>();
		if (view != null) {
			NetworkViewID viewID = Network.AllocateViewID();
			view.viewID = viewID;
		}
	}
}
