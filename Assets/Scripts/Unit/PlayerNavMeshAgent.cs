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
		if (Input.GetMouseButton(1) && selection.isSelected && this.playerNetworkView.isMine) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				agent.SetDestination(hit.point);
			}
		}
	}

	public NavMeshAgent getAgent(){
		return agent;
	}
}
