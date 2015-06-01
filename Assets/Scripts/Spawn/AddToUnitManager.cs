using UnityEngine;
using System.Collections;

public class AddToUnitManager : MonoBehaviour {
	public void Awake() {
		UnitManager.instance.AllUnits.Add(this.gameObject);
	}
}
