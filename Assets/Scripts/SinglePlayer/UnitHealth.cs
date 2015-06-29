using UnityEngine;
using System.Collections;

public class UnitHealth : MonoBehaviour {
	public int healthPoints;
	public int initialHealthPoints = 10;

	private void Start() {
		this.healthPoints = this.initialHealthPoints;
	}

	public void TakeDamage() {
		if (this.healthPoints > 0) {
			Debug.Log("Taking damage. Current health point: " + this.healthPoints);
			this.healthPoints--;
			if (this.healthPoints <= 0) {
				Debug.Log("Unit has died.");
				this.SendMessage("Deactivate");
			}
		}
	}
}
