using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerUnitManager : MonoBehaviour {
	public List<GameObject> SelectableUnits;
	public static PlayerUnitManager Instance;

	private void Awake() {
		PlayerUnitManager.Instance = this;
		this.SelectableUnits = new List<GameObject>();
	}
}
