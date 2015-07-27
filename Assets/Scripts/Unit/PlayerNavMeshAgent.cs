using UnityEngine;
using System.Collections;

public class PlayerNavMeshAgent : MonoBehaviour {
	Selectable selection;
	NavMeshAgent agent;
	private NetworkView playerNetworkView;

	// Use this for initialization
	void Start () {
		this.agent = this.GetComponent<NavMeshAgent>();
		this.selection = this.GetComponentInChildren<Selectable>();
		this.playerNetworkView = this.GetComponent<NetworkView>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1)) {
			if ((selection.isSelected) && this.playerNetworkView.isMine) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					this.playerNetworkView.RPC("RPC_SetTarget", RPCMode.AllBuffered, hit.point);
				}
			}
		}
	}

	public NavMeshAgent getAgent(){
		return this.agent;
	}

	[RPC]
	public void RPC_SetTarget(Vector3 target) {
		//if (this.playerNetworkView.isMine) {
			this.agent.SetDestination(target);
		//}
	}
}
