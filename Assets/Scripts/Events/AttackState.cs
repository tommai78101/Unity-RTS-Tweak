using UnityEngine;
using System.Collections;

public class AttackState : MonoBehaviour {
	private StateChangeable StateChangeable;

	public void Awake() {
		StateChangeable = this.GetComponent<StateChangeable>();
		if (StateChangeable != null){
			StateChangeable.EventStateChange += State_Attack;
		}
	}

	public void OnDisable() {
		if (StateChangeable != null) {
			StateChangeable.EventStateChange -= State_Attack;
		}
	}

	public void State_Attack(StateChangeable state, StateChangeable.StateType type){
		if (type == StateChangeable.StateType.Attack) {
			Debug.Log("Attacking....");
		}
	}
}
