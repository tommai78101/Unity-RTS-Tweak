using UnityEngine;
using System.Collections;

public class AddToUnitManager : MonoBehaviour {
	public void Awake() {
		UnitManager.instance.PlayerUnits.Add(this.gameObject);
	}
}
