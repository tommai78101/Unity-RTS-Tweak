using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public float healthPercentage;
	public int maxHealth;
	public int currentHealth;
	public float redCountdown;
	public float healingTime;

	private float healingCooldown;
	private Material material;
	private Color initialColor;
	private bool isTakingDamage = false;

	public void Start() {
		Renderer renderer = this.GetComponent<Renderer>();
		this.material = renderer.material;
		this.initialColor = this.material.color;
		this.redCountdown = 0f;
		this.isTakingDamage = false;
		if (this.healingTime < 3f) {
			this.healingTime = 3f;
		}
		this.healingCooldown = 0f;
	}

	public void DecreaseHealth(int attackPower) {
		this.currentHealth -= attackPower;
		FlashRed();
		if (this.currentHealth < 0) {
			this.currentHealth = 0;
			this.healthPercentage = 0f;
		}
		else {
			this.healthPercentage = (float) this.currentHealth / (float) this.maxHealth;
		}
	}

	public void IncreaseHealth(int healPower) {
		this.currentHealth += healPower;
		if (this.currentHealth > this.maxHealth) {
			this.currentHealth = this.maxHealth;
			this.healthPercentage = 1f;
		}
		else {
			this.healthPercentage = (float) this.currentHealth / (float) this.maxHealth;
		}
	}

	private void OnGUI() {
		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.black;
		style.alignment = TextAnchor.MiddleCenter;
		Vector3 healthPosition = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
		Rect healthRect = new Rect(healthPosition.x - 50f, (Screen.height - healthPosition.y) - 45f, 100f, 25f);
		GUI.Label(healthRect, this.currentHealth.ToString() + "/" + this.maxHealth.ToString(), style);
	}

	public void Copy(HealthBar other) {
		this.currentHealth = other.currentHealth;
		this.maxHealth = other.maxHealth;
		this.healthPercentage = other.healthPercentage;
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
		HealUpdate();
	}

	private void HealUpdate() {
		if (this.healthPercentage < 1f || this.currentHealth < this.maxHealth) {
			if (this.healingCooldown < 1f) {
				this.healingCooldown += Time.deltaTime / this.healingTime;
			}
			else {
				this.healingCooldown = 0f;
				IncreaseHealth(1);
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
