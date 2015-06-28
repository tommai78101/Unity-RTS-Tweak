using UnityEngine;
using System.Collections;

public enum UnitState {
	IDLE,
	ATTACK,
	SCOUTING,
	DIVIDING,
	MERGING,
	DYING,
	WAITING_FOR_ORDERS
}

public enum UnitCommand {
	ATTACK_ORDER,
	SPLIT_ORDER,
	MERGE_ORDER,
	MOVE_ORDER,
	NO_ORDERS
}

public class UnitStateManager : MonoBehaviour {
	public UnitState actionState;
	public bool selectFlag;
	public UnitCommand commandState;

	private GameObject attackee;
	private NavMeshAgent agent;

	private void Start() {
		this.actionState = UnitState.IDLE;
		this.agent = this.GetComponent<NavMeshAgent>();
		if (this.agent == null) {
			Debug.LogError(new System.NullReferenceException("No nav mesh agent detected."));
		}
		this.agent.updateRotation = true;
		this.agent.stoppingDistance = 0.85f;
		this.selectFlag = false;
		this.commandState = UnitCommand.NO_ORDERS;
	}

	private void Update() {
		if (this.selectFlag && Input.GetMouseButtonUp(1) && this.actionState.Equals(UnitCommand.NO_ORDERS)) {
			this.actionState = UnitState.WAITING_FOR_ORDERS;
			return;
		}
		switch (this.actionState) {
			default:
			case UnitState.WAITING_FOR_ORDERS:
				WaitingOnOrders();
				break;
			case UnitState.ATTACK:
				AttackAction();
				break;
			case UnitState.DIVIDING:
				DivideAction();
				break;
			case UnitState.DYING:
				DeathAction();
				break;
			case UnitState.IDLE:
				IdleAction();
				break;
			case UnitState.MERGING:
				MergeAction();
				break;
			case UnitState.SCOUTING:  //Moving
				ScoutAction();
				break;
		}
	}

	private void AttackAction() {
		if (this.attackee != null) {
			if (!this.agent.pathPending) {
				if (this.agent.remainingDistance <= this.agent.stoppingDistance) {
					if (this.agent.hasPath || this.agent.velocity.sqrMagnitude == 0f) {
						//Stopped.
						float distance = Vector3.Distance(this.transform.position, this.attackee.transform.position);
						if (distance >= 2.3f) {
							this.agent.SetDestination(this.attackee.transform.position);
						}
					}
				}
			}
			//Do attack here.
			UnitHealth health = this.attackee.GetComponent<UnitHealth>();
			if (health != null && health.healthPoints > 0) {
				this.attackee.SendMessage("DoDamage");
			}
			return;
		}
	}

	private void AttackObject(GameObject victim) {
		Debug.Log("Attacking the " + victim.name.ToString());
		this.actionState = UnitState.ATTACK;
		Vector3 spacing = victim.transform.position - this.transform.position;
		spacing *= 0.8f;
		spacing += this.transform.position;
		this.agent.SetDestination(spacing);
	}

	private void DivideAction() {
	}

	private void DeathAction() {
	}

	private void IdleAction() {
		CheckSurroundings();
	}

	private void MergeAction() {
	}

	private void ScoutAction() {

	}

	private void WaitingOnOrders() {
		if (this.commandState.Equals(UnitCommand.NO_ORDERS) && this.actionState.Equals(UnitState.WAITING_FOR_ORDERS)) {
			if (Input.GetKeyUp(KeyCode.A)) {
				this.commandState = UnitCommand.ATTACK_ORDER;
				Debug.Log("ATTACK ORDER");
			}
			else if (Input.GetKeyUp(KeyCode.S)) {
				this.commandState = UnitCommand.SPLIT_ORDER;
				Debug.Log("SPLIT ORDER");
			}
			else if (Input.GetKeyUp(KeyCode.D)) {
				this.commandState = UnitCommand.MERGE_ORDER;
				Debug.Log("MERGE ORDER");
			}
			else if (Input.GetMouseButtonUp(1)) {
				this.commandState = UnitCommand.MOVE_ORDER;
				Debug.Log("MOVE ORDER");
			}
			else if (Input.GetMouseButtonUp(0)) {
				this.commandState = UnitCommand.NO_ORDERS;
				this.actionState = UnitState.IDLE;
				Debug.Log("CANCEL ORDER");
			}
		}
	}

	private void CheckSurroundings() {
		//Check if enemies are nearby.
		bool currentState = this.actionState.Equals(UnitState.IDLE) || this.actionState.Equals(UnitState.WAITING_FOR_ORDERS) || this.actionState.Equals(UnitState.ATTACK);
		if (currentState) {
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f);
			bool enemyNearby = false;
			for (int i = 0; i <colliders.Length; i++){
				if (!colliders[i].name.Equals("Floor") && !colliders[i].gameObject.Equals(this.gameObject)){
					float distance = Vector3.Distance(colliders[i].transform.position, this.transform.position);
					Debug.Log("Distance: " + distance);
					if (distance < 1f) {
						this.attackee = colliders[i].gameObject;
						this.SendMessage("AttackObject", this.attackee);
						enemyNearby = true;
						break;
					}
				}
			}
			if (this.attackee != null && !enemyNearby) {
				this.attackee = null;
			}
		}
	}
}