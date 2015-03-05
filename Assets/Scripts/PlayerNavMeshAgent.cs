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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)){
				agent.SetDestination(hit.point);
			}
		}
	}

	public NavMeshAgent getAgent(){
		return agent;
	}
}
