using UnityEngine;
using System.Collections;

public enum UnitState {
	IDLE,
	ATTACK,
	SCOUTING,
	DIVIDING,
	MERGING,
	DYING
}

public class UnitStateManager : MonoBehaviour {
	public UnitState state;
	public bool selectFlag;

	private void Start() {
		this.state = UnitState.IDLE;
	}

	private void Update() {
		switch (this.state) {
			case UnitState.ATTACK: {
					break;
				}
			case UnitState.DIVIDING: {
					break;
				}
			case UnitState.DYING: {
					break;
				}
			case UnitState.IDLE: {

					break;
				}
			case UnitState.MERGING: {
					break;
				}
			case UnitState.SCOUTING: { //Moving
					break;
				}
		}
	}
}