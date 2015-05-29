using UnityEngine;
using System.Collections;

public class UnitNavMeshAgent : MonoBehaviour {

	NavMeshAgent agent;
	bool attackFlag;

	public PlayerNavMeshAgent target;
	public float attackRange;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			NetworkView networkView = this.GetComponent<NetworkView>();
			if (networkView != null) {
				networkView.RPC("RPC_Move", RPCMode.AllBuffered, null);
			}
		}
	}

	[RPC]
	public void RPC_Move() {
		Vector3 targetPos = target.transform.position;
		Vector3 pos = this.transform.position;

		if (attackFlag && Vector3.Distance(targetPos, pos) > attackRange) {
			attackFlag = false;
		}
		else if (agent.remainingDistance < 0.3f || Vector3.Distance(targetPos, pos) < attackRange) {
			attackFlag = true;
		}

		if (attackFlag) {
			agent.SetDestination(target.transform.position);
		}
		else {
			agent.SetDestination(Vector3.zero);
		}
	}
}
