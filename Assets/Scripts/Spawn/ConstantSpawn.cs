using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConstantSpawn : MonoBehaviour {
	public Transform DefaultSpawnLocation;

	private int SpawnCounter = 0;
	private float StartTime = 0.0f;

	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (Network.isClient || Network.isServer) {
			if (Time.time - StartTime >= 5f) {
				StartTime = Time.time;

				this.StartCoroutine(CR_SpawnTempUnit());
			}
		}
	}

	public IEnumerator CR_SpawnTempUnit() {
		Vector3 spawnLocation = this.DefaultSpawnLocation.position;
		bool tryAgain = true;
		while (tryAgain) {
			tryAgain = false;
			Collider[] colliders = Physics.OverlapSphere(spawnLocation, 0.6f);
			foreach (Collider c in colliders) {
				if (c.gameObject.name.Equals("Floor")) {
					continue;
				}
				if (c.gameObject.tag.Equals("TemporaryUnit") || c.gameObject.tag.Equals("Player") || c.gameObject.name.Equals("EntityBody")) {
					tryAgain = true;
					break;
				}
			}
			if (tryAgain) {
				spawnLocation.x += 1.05f;
			}
		}

		GameObject spawnedObject = (GameObject)Network.Instantiate(Resources.Load("Prefabs/Player"), spawnLocation, Quaternion.identity, UnitManager.PG_Player);
		spawnedObject.name = spawnedObject.name + " " + string.Format("{0}", this.SpawnCounter);
		spawnedObject.tag = "Player";

		this.SpawnCounter++;
		yield return null;
	}


	public void OnCollisionEnter(UnityEngine.Collision col) {
		Debug.Log("OnCollisionEnter: " + col.gameObject.ToString());
	}
}
