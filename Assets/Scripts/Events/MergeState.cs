using UnityEngine;
using System.Collections;

public class MergeState : MonoBehaviour {
	private StateChangeable StateChangeable;

	public void Awake() {
		StateChangeable = this.GetComponent<StateChangeable>();
		if (StateChangeable != null){
			StateChangeable.EventStateChange += State_Merge;
		}
	}

	public void OnDisable() {
		if (StateChangeable != null) {
			StateChangeable.EventStateChange -= State_Merge;
		}
	}

	public void State_Merge(StateChangeable state, StateChangeable.StateType type){
		if (type == StateChangeable.StateType.Merge) {
			Debug.Log("Merging....");
		}
	}
}
