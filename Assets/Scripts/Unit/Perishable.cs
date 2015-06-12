using UnityEngine;
using System.Collections;

public class Perishable : MonoBehaviour {
	public void OnDestroy() {
		if (this.gameObject != null) {
			Debug.Log("Removing traces of this game object from unit manager. Object: " + this.gameObject.name);
			UnitManager.Instance.PlayerUnits.Remove(this.gameObject);
			UnitManager.Instance.AllUnits.Remove(this.gameObject);
			Debug.Log("Current unit count: " + UnitManager.Instance.PlayerUnits.Count.ToString());
		}
	}
}
