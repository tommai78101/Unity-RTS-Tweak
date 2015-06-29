using UnityEngine;
using System.Collections;

public class UnitHealth : MonoBehaviour {
	public int healthPoints;
	public int initialHealthPoints = 10;

	private float redCountdown;
	private Color initialColor;
	private Material material;
	private bool isTakingDamage = false;

	private void Start() {
		this.healthPoints = this.initialHealthPoints;
		Renderer renderer = this.GetComponent<Renderer>();
		if (renderer != null) {
			this.initialColor = renderer.material.color;
			this.material = renderer.material;
		}
		this.redCountdown = 0f;
		this.isTakingDamage = false;
	}

	public void TakeDamage() {
		if (this.healthPoints > 0) {
			Debug.Log("Taking damage. Current health point: " + this.healthPoints);
			this.healthPoints--;
			FlashRed();
			if (this.healthPoints <= 0) {
				Debug.Log("Unit has died.");
				this.SendMessage("Deactivate");
			}
		}
	}

	private void Update() {
		if (this.material != null && this.isTakingDamage) {
			if (this.redCountdown > 0f) {
				this.redCountdown -= Time.deltaTime;
				this.material.color = Color.Lerp(Color.red, this.initialColor, 1f - this.redCountdown);
			}
			else {
				this.material.color = this.initialColor;
				this.isTakingDamage = false;
			}
		}
	}

	private void FlashRed() {
		if (this.material != null) {
			this.redCountdown = 1f;
			this.isTakingDamage = true;
		}
	}
}
