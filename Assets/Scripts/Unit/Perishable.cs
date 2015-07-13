using UnityEngine;
using System.Collections;

public class Perishable : MonoBehaviour {
	public void OnDestroy() {
		if (UnitManager.Instance != null && this.gameObject != null) {
			Debug.Log("Removing traces of this game object from unit manager. Object: " + this.gameObject.name);
			UnitManager.Instance.PlayerUnits.Remove(this.gameObject);
			UnitManager.Instance.AllUnits.Remove(this.gameObject);
			Debug.Log("Current unit count: " + UnitManager.Instance.PlayerUnits.Count.ToString());

			if (UnitManager.Instance.PlayerUnits.Count <= 0) {
				WinLoseCondition condition = UnitManager.Instance.gameObject.GetComponent<WinLoseCondition>();
				if (condition != null) {
					Debug.Log("WinLoseCondition found! Calling on event and delegate triggers.");
					condition.PlayerHasLost();
				}
			}
		}
	}
}
