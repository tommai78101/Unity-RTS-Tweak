using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/*

TODO: This is for Leveling, Health, Attack Power, and Death.

*/

public class TutorialAttribute : NetworkBehaviour {
	public int level;
	public int attackPower;
	public int maxHealth;
	public int currentHealth;
	public float attackRadius;


	void Start () {
		this.level = 1;
		this.attackPower = 1;
		this.maxHealth = 5;
		this.currentHealth = 5;

		Renderer renderer = this.GetComponent<Renderer>();
		Vector3 size = renderer.bounds.size;

		this.attackRadius = 2.5f + ((size / 2f).magnitude);
	}
}
