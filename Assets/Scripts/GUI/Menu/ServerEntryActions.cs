using UnityEngine;
using System.Collections;

public class ServerEntryActions : MonoBehaviour {
	public HostData data;

	public void ConnectToServer() {
		if (data != null) {
			Debug.Log("Connecting to server with data: " + data.ToString());
			Network.Connect(data);
			Debug.Log("Now loading level.");
			Application.LoadLevel("test_nav");
		}
		else {
			Debug.Log("Host data is null. Cannot connect at all.");
		}
	}
}
