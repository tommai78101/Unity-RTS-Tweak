using UnityEngine;
using System.Collections;

public class AddToUnitManager : MonoBehaviour {
	public void Awake() {
		UnitManager.Instance.AllUnits.Add(this.gameObject);
		NetworkView networkView = this.GetComponent<NetworkView>();
		if (networkView != null && networkView.isMine) {
			UnitManager.Instance.PlayerUnits.Add(this.gameObject);
		}
	}
}
