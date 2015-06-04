using UnityEngine;
using System.Collections;

public class AddToUnitManager : MonoBehaviour {
	public void Awake() {
		UnitManager.instance.AllUnits.Add(this.gameObject);
		NetworkView networkView = this.GetComponent<NetworkView>();
		if (networkView != null && networkView.isMine) {
			UnitManager.instance.PlayerUnits.Add(this.gameObject);
		}
	}
}
