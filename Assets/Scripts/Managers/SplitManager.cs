using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SpawnGroup {
	public GameObject owner;
	public GameObject spawnedUnit;
	public float elapsedTime;
	public Vector3 centerPosition;
	public Vector3 ownerTarget;
	public Vector3 spawnTarget;
	public float cooldownTime;

	public SpawnGroup(GameObject owner, GameObject unit, float cooldownTime, float randomValue) {
		this.owner = owner;
		this.spawnedUnit = unit;
		this.elapsedTime = 0f;
		this.ownerTarget = this.spawnTarget = Vector3.zero;
		this.centerPosition = owner.transform.position;
		this.cooldownTime = cooldownTime;

		Renderer renderer = this.owner.GetComponent<Renderer>();
		if (renderer != null) {
			Vector3 size = renderer.bounds.size;
			size.y = size.z = 0f;
			Vector3 rotation = Quaternion.Euler(0f, randomValue, 0f) * (size / 2f);
			this.ownerTarget = new Vector3(this.centerPosition.x + rotation.x, this.centerPosition.y, this.centerPosition.z + rotation.z);
			this.spawnTarget = new Vector3(this.centerPosition.x - rotation.x, this.centerPosition.y, this.centerPosition.z - rotation.z);
		}
	}
};

public class SplitManager : MonoBehaviour {
	public List<SpawnGroup> spawnGroups;
	public List<SpawnGroup> remove;

	private void Start() {
		this.spawnGroups = new List<SpawnGroup>();
		this.remove = new List<SpawnGroup>();
	}

	private void Update() {
		if (this.spawnGroups.Count > 0) {
			for (int i = 0; i < this.spawnGroups.Count; i++) {
				MoveToPosition(i);
				Cooldown(i);
				UpdateGroup(i);
			}
			foreach (SpawnGroup group in this.spawnGroups) {
				if (group.elapsedTime > 1f) {
					this.remove.Add(group);
				}
			}
		}
		if (this.remove.Count > 0) {
			foreach (SpawnGroup group in this.remove) {
				Divisible div = group.owner.GetComponent<Divisible>();
				if (!div.IsDivisibleStateReady()) {
					div.SetDivisibleReady();
				}
				div = group.spawnedUnit.GetComponent<Divisible>();
				if (!div.IsDivisibleStateReady()) {
					div.SetDivisibleReady();
				}
				this.spawnGroups.Remove(group);
			}
			this.remove.Clear();
		}
	}

	//----------------------------------------------------------------------------------------

	private void UpdateGroup(int index) {
		SpawnGroup group = this.spawnGroups[index];
		group.elapsedTime += Time.deltaTime / group.cooldownTime;
		this.spawnGroups[index] = group;
	}

	private void MoveToPosition(int index) {
		SpawnGroup group = this.spawnGroups[index];
		if (group.elapsedTime < 1f) {
			GameObject owner = group.owner;
			GameObject spawn = group.spawnedUnit;
			owner.transform.position = Vector3.Lerp(group.centerPosition, group.ownerTarget, group.elapsedTime);
			spawn.transform.position = Vector3.Lerp(group.centerPosition, group.spawnTarget, group.elapsedTime);
		}
	}

	private void Cooldown(int index) {
		SpawnGroup group = this.spawnGroups[index];
		Renderer rendererA = group.owner.GetComponentInChildren<Renderer>();
		Renderer rendererB = group.spawnedUnit.GetComponentInChildren<Renderer>();
		if (group.elapsedTime < 1f) {
			bool halfTime = group.elapsedTime < 0.5f;
			if (halfTime) {
				rendererA.material.color = Color.Lerp(Color.white, Color.cyan, group.elapsedTime);
				rendererB.material.color = Color.Lerp(Color.white, Color.cyan, group.elapsedTime);
			}
			else {
				rendererB.material.color = Color.Lerp(Color.cyan, Color.white, group.elapsedTime);
				rendererA.material.color = Color.Lerp(Color.cyan, Color.white, group.elapsedTime);
			}
		}
		else if (group.elapsedTime >= 1f) {
			Selectable selectable = group.owner.GetComponent<Selectable>();
			if (selectable != null && !selectable.IsSelectionEnabled()) {
				selectable.EnableSelection();
			}
			selectable = group.spawnedUnit.GetComponent<Selectable>();
			if (selectable != null && !selectable.IsSelectionEnabled()) {
				selectable.EnableSelection();
			}
			rendererA.material.color = Color.white;
			rendererB.material.color = Color.white;
		}
	}
}
