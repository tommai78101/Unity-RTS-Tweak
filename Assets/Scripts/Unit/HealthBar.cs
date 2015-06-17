using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public float healthPercentage;
	public int maxHealth;
	public int currentHealth;

	//public void Start() {
	//	this.currentHealth = this.maxHealth;
	//	this.healthPercentage = (float) this.currentHealth / (float) this.maxHealth;
	//}

	public void DecreaseHealth(int attackPower) {
		this.currentHealth -= attackPower;
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
}
