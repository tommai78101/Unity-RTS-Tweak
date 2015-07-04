using UnityEngine;
using System.Collections;

public class ServerEntryActions : MonoBehaviour {
	public HostData data;

	public void ConnectToServer() {
		if (data != null) {
			Debug.Log("Connecting to server with data: " + data.ToString());
			NetworkConnectionError error = Network.Connect(data);
			if (error != NetworkConnectionError.NoError) {
				error = Network.Connect("localhost", 12345);
				if (error != NetworkConnectionError.NoError) {
					Debug.Log("Server is probably not hosted, or the connection is blocked.");
				}
			}
			Debug.Log("Now loading level.");
			Application.LoadLevel("test_nav");
		}
		else {
			Debug.Log("Host data is null. Cannot connect at all.");
		}
	}
}
