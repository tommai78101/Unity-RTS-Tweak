using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * TODO: Add labels and directions for control. How-to-play-this-game?
 */


public struct BattlePair {
	public GameObject attacker;
	public GameObject attackee;
	public bool isIgnored;

	public BattlePair(GameObject a, GameObject b) {
		this.attacker = a;
		this.attackee = b;
		this.isIgnored = false;
	}

	public void SetIgnored() {
		this.isIgnored = true;
	}
};

public class Attackable : MonoBehaviour {
	private Selectable selectable;
	private Renderer attackableRenderer;
	private NetworkView attackableNetworkView;
	private NavMeshAgent agent;

	public Vector3 attackTargetPosition;
	public bool isReadyToAttack;
	public bool isAttacking;
	private bool receivedAttackCommand;
	public int strength;
	public List<GameObject> attackTargetUnits = new List<GameObject>();

	/*
	 * TODO: Build a mouse cursor icon:
	 *    - 32x32
	 *    - http://answers.unity3d.com/questions/364766/creating-a-custom-cursor.html
	 *    - http://answers.unity3d.com/questions/145024/unity3d-custom-cursors.html
	 *    - http://answers.unity3d.com/questions/38497/custom-cursor-how-does-it-work.html
	 * 
	 * TODO: Use color scheme to determine health.
	 * 
	 * TODO: Stop all lists from using GameObject as type. Use corresponding MonoBehaviour types 
	 *       for associated types (List in Selectable -> Use Selectable as type).
	 */

	public void Start() {
		this.selectable = this.GetComponent<Selectable>();
		if (this.selectable == null) {
			Debug.LogException(new System.NullReferenceException("Selectable is not supposed to be null. Probably an incorrect placement of the script."));
		}
		this.attackableRenderer = this.GetComponent<Renderer>();
		if (this.attackableRenderer == null) {
			Debug.LogException(new System.NullReferenceException("Renderer is not supposed to be null. If it's null, I don't know what is wrong."));
		}
		this.attackableNetworkView = this.GetComponent<NetworkView>();
		if (this.attackableNetworkView == null) {
			Debug.LogException(new System.NullReferenceException("NetworkView is not supposed to be null. Probably forgot to put a network view on the object."));
		}
		this.agent = this.GetComponent<NavMeshAgent>();
		if (this.agent == null) {
			Debug.LogException(new System.NullReferenceException("NavMeshAgent is not supposed to be null. Check to make sure agents are attached as components."));
		}
		this.isReadyToAttack = false;
		this.isAttacking = false;
		this.receivedAttackCommand = false;
		this.strength = 1;
	}

	public void OnGUI() {
		if (this.isReadyToAttack && !this.isAttacking) {
			if (Input.GetMouseButtonUp(1)) {
				//Right click
				this.isReadyToAttack = false;
				this.isAttacking = true;
				this.receivedAttackCommand = false;
			}
			else if (this.selectable.isSelected && (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))) {
				//Left click
				this.isReadyToAttack = false;
				this.isAttacking = false;
				this.selectable.Deselect();
				this.attackableRenderer.material.color = Color.white;
			}
		}
		else if (this.isAttacking) {
			if (Input.GetMouseButtonUp(1) && this.selectable.isSelected && !this.isReadyToAttack && this.receivedAttackCommand) {
				Debug.Log("Cancelling attack.");
				this.isAttacking = false;
				this.receivedAttackCommand = false;
			}
		}
		else if (Input.GetKeyUp(KeyCode.A) && this.selectable.isSelected && this.attackableNetworkView.isMine && !this.isReadyToAttack && !this.isAttacking) {
			this.isReadyToAttack = true;
			this.attackableRenderer.material.color = Color.yellow;
		}
	}

	public void Update() {
		if (this.attackableNetworkView.isMine) {
			if (this.isReadyToAttack && !this.isAttacking) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				Physics.Raycast(ray, out hit);
				Debug.DrawLine(this.gameObject.transform.position, hit.point);
			}
			else if (!this.isReadyToAttack && this.isAttacking && !this.receivedAttackCommand) {
				if (this.attackableNetworkView != null) {
					this.receivedAttackCommand = true;
					this.attackableNetworkView.RPC("RPC_Attack", RPCMode.OthersBuffered, Input.mousePosition);
				}
			}
		}
		CheckForEnemies();
		AttackEnemies();
	}

	[RPC]
	public void RPC_Attack(Vector3 targetPosition) {
		if (Network.isClient) {
			Debug.Log("RPC_Attack has been called. Vector: " + targetPosition.ToString());
			this.attackTargetPosition = targetPosition;
			this.agent.SetDestination(this.attackTargetPosition);
		}
		else {
			Debug.LogError("Client RPC_Attack was not called.");
		}
	}

	private void CheckForEnemies() {
		if (UnitManager.Instance == null) {
			return;
		}
		for (int i = 0; i < UnitManager.Instance.AllUnits.Count; i++) {
			GameObject o = UnitManager.Instance.AllUnits[i];
			if (o == null) {
				continue;
			}
			if (UnitManager.Instance.PlayerUnits.Contains(o) || this.gameObject.name.Equals(o.name)) {
				continue;
			}
			NetworkView view = o.GetComponent<NetworkView>();
			if (!view.isMine) {
				if ((Vector3.Distance(this.gameObject.transform.position, o.transform.position) < 4f) && !this.attackTargetUnits.Contains(o)) {
					this.attackTargetUnits.Add(o);
				}
			}
		}
	}

	private void AttackEnemies() {
		if (this.attackTargetUnits.Count > 0) {
			GameObject enemy = this.attackTargetUnits[0];
			if (enemy == null) {
				this.attackTargetUnits.RemoveAt(0);
				return;
			}
			Selectable enemySelect = enemy.GetComponent<Selectable>();
			if (enemySelect == null) {
				this.attackTargetUnits.RemoveAt(0);
				return;
			}
			if (enemySelect.UUID.Equals(this.selectable.UUID)) {
				this.attackTargetUnits.RemoveAt(0);
				return;
			}
			DeathCheck check = enemy.GetComponent<DeathCheck>();
			if (check != null && !check.isDead) {
				if (Vector3.Distance(enemy.transform.position, this.gameObject.transform.position) <= 2.5f) {
					if (this.gameObject != null && enemy.gameObject != null) {
						BattlePair p = new BattlePair(this.gameObject, enemy);
						if (!DeathCheck.pairs.Contains(p)) {
							Attackable attack = enemy.GetComponent<Attackable>();
							if (attack != null) {
								if (attack.GetStrength() <= 0) {
									check.Kill();
									DeathCheck.pairs.Add(p);
								}
								else {
									attack.DecreaseStrength();
								}
							}
						}
					}
				}
				else {
					Vector3 unitVector = (enemy.transform.position - this.gameObject.transform.position).normalized;
					if (this.attackableNetworkView != null && !this.receivedAttackCommand) {
						this.receivedAttackCommand = true;
						this.attackableNetworkView.RPC("RPC_Attack", RPCMode.AllBuffered, this.gameObject.transform.position + unitVector);
					}
				}
			}
		}
	}

	public void StopAttacking() {
		this.isAttacking = false;
		this.isReadyToAttack = false;
	}

	public void IncreaseStrength() {
		this.strength++;
	}

	public void DecreaseStrength() {
		this.strength--;
	}

	public int GetStrength() {
		return this.strength;
	}
}
