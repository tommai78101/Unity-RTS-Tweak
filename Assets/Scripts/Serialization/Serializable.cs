using UnityEngine;
using System.Collections;

public class Serializable : MonoBehaviour {
	private void OnSerializeNetworkView(BitStream Stream, NetworkMessageInfo Info) {
		if (Stream.isWriting) {
			Vector3 pos = this.transform.position;
			Stream.Serialize(ref pos);
		}
		else {
			Vector3 pos = Vector3.zero;
			Stream.Serialize(ref pos);
			this.transform.position = pos;
		}
	}
}
