using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	public PlayerNavMeshAgent target;
	public Vector3 camPosition;

	// Use this for initialization
	void Start () {
		this.transform.position = camPosition;
	}
	
	// Update is called once per frame
	void Update () {
		NavMeshAgent agent = target.getAgent ();
		Vector3 pos = agent.transform.position;
		pos.x += camPosition.x;
		pos.y = camPosition.y;
		pos.z += camPosition.z;
		this.transform.position = pos;
	}
}
