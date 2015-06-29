using UnityEngine;
using System.Collections;

public class UnitHealth : MonoBehaviour {
	public int healthPoints;
	public int initialHealthPoints = 10;

	private float redCountdown;
	private Color initialColor;
	private Material material;

	private void Start() {
		this.healthPoints = this.initialHealthPoints;
		Renderer renderer = this.GetComponent<Renderer>();
		if (renderer != null) {
			this.initialColor = renderer.material.color;
			this.material = renderer.material;
		}
		this.redCountdown = 0f;
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
		if (this.material != null) {
			if (this.redCountdown > 0f) {
				this.material.color = Color.Lerp(this.initialColor, Color.red, this.redCountdown);
			}
			else {
				this.redCountdown -= Time.deltaTime / 3f;
			}
		}
	}

	private void FlashRed() {
		if (this.redCountdown <= 0f) {
			this.redCountdown = 1f;
		}
	}
}
