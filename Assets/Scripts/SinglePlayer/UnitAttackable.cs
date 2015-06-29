using UnityEngine;
using System.Collections;

public class UnitAttackable : MonoBehaviour {
	const float initialCooldown = 2.317f;

	public float cooldown;

	private bool activateFlag;

	public void SetCooldown() {
		this.cooldown = UnitAttackable.initialCooldown;
	}

	public void Activate() {
		this.activateFlag = true;
	}

	public void Deactivate() {
		this.activateFlag = false;
	}

	public void DoDamage() {
		if (this.cooldown > 0) {
			this.cooldown -= Time.deltaTime;
		}
		else {
			Debug.Log("Attacked");
			this.SendMessage("TakeDamage");
			this.cooldown = UnitAttackable.initialCooldown;
		}
	}
}
