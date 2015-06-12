using UnityEngine;
using System.Collections;

public class DeathState : MonoBehaviour {
	private StateChangeable StateChangeable;

	public void Awake() {
		StateChangeable = this.GetComponent<StateChangeable>();
		if (StateChangeable != null) {
			StateChangeable.EventStateChange += State_Death;
		}
	}

	public void OnDisable() {
		if (StateChangeable != null) {
			StateChangeable.EventStateChange -= State_Death;
		}
	}

	public void State_Death(StateChangeable state, StateChangeable.StateType type) {
		if (type == StateChangeable.StateType.Death) {
			Debug.Log("Dying....");
		}
	}

	public void State_Death_Trigger() {

	}

	public void State_Death_Update() {

	}
}
