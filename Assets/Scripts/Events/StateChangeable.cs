using UnityEngine;
using System.Collections;

public class StateChangeable : MonoBehaviour {
	public enum StateType {
		Divide, Merge, Select, Attack, Death, Idle, WaitOrders
	};

	//System.Action<> is a delegate.
	public event System.Action<StateChangeable, StateType> EventStateChange;
	public event System.Action<StateChangeable> EventTrigger;
	public event System.Action<StateType> EventUpdate;
	private StateType stateType;

	public virtual StateType State {
		get {
			return stateType;
		}
		set {
			if (stateType == value) {
				return;
			}
			stateType = value;

			if (this.EventStateChange != null) {
				this.EventStateChange(this, stateType);
			}
		}
	}

	public void Awake() {
		this.State = StateType.Select;
	}

	public void OnGUI() {
		if (this.EventTrigger != null) {
			this.EventTrigger(this);
		}
	}

	public void Update() {
		if (this.EventUpdate != null) {
			this.EventUpdate(this.State);
		}
	}
}
