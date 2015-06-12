using UnityEngine;
using System.Collections;

public class DivideState : MonoBehaviour {
	private StateChangeable StateChangeable;

	public void Awake() {
		StateChangeable = this.GetComponent<StateChangeable>();
		if (StateChangeable != null){
			StateChangeable.EventStateChange += State_Divide;
		}
	}

	public void OnDisable() {
		if (StateChangeable != null) {
			StateChangeable.EventStateChange -= State_Divide;
		}
	}

	public void State_Divide(StateChangeable state, StateChangeable.StateType type){
		if (type == StateChangeable.StateType.Divide) {
			Debug.Log("Dividing....");
		}
	}
}
