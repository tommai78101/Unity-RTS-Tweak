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
		if (this.selectFlag) {
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
			HandleOrders();
		}
	}

	private void AttackAction() {
		if (this.attackee != null) {
			if (!this.agent.pathPending) {
				if (this.agent.remainingDistance <= this.agent.stoppingDistance) {
					if (this.agent.hasPath || this.agent.velocity.sqrMagnitude == 0f) {
						//Stopped.
						float distance = Vector3.Distance(this.transform.position, this.attackee.transform.position);
						if (distance >= 1f) {
							this.agent.stoppingDistance = 0.9f;
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
		ReachingDestination();
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
				Move();
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
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f);
			bool enemyNearby = false;
			for (int i = 0; i < colliders.Length; i++) {
				if (!colliders[i].name.Equals("Floor") && !colliders[i].gameObject.Equals(this.gameObject)) {
					float distance = Vector3.Distance(colliders[i].transform.position, this.transform.position);
					if (distance < 4f) {
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

	private void HandleOrders() {
		switch (this.commandState) {
			case UnitCommand.NO_ORDERS:
				if (Input.GetMouseButtonUp(1)) {
					Move();
				}
				else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) {
					this.commandState = UnitCommand.NO_ORDERS;
					this.actionState = UnitState.WAITING_FOR_ORDERS;	
				}
				break;
			case UnitCommand.MOVE_ORDER:
				if (Input.GetMouseButtonUp(1)) {
					Move();
				}
				break;
			default:
				break;
		}
	}

	private void Move() {
		Debug.Log("Giving attack order and setting new destination.");
		this.actionState = UnitState.SCOUTING;
		this.commandState = UnitCommand.MOVE_ORDER;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo)) {
			Debug.Log("New move target position: " + hitInfo.point);
			this.agent.SetDestination(hitInfo.point);
		}
	}

	private void ReachingDestination() {
		if (!this.agent.pathPending) {
			if (this.agent.remainingDistance <= this.agent.stoppingDistance) {
				if (this.agent.hasPath || this.agent.velocity.sqrMagnitude == 0f) {
					this.actionState = UnitState.IDLE;
					this.commandState = UnitCommand.NO_ORDERS;
				}
			}
		}
	}
}