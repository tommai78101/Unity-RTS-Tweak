using UnityEngine;
using System.Collections;

public class PlayerNavMeshAgent : MonoBehaviour {
	Selectable selection;
	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		selection = this.GetComponentInChildren<Selectable>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1) && selection.isSelected){
			NetworkView networkView = this.GetComponent<NetworkView>();
			if (networkView != null) {
				networkView.RPC("RPC_Move", RPCMode.AllBuffered, null);
			}
		}
	}

	public NavMeshAgent getAgent(){
		return agent;
	}

	[RPC]
	public void RPC_Move() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			agent.SetDestination(hit.point);
		}
	}
}
