﻿using UnityEngine;
using System.Collections;

public struct SpawnUnit {
	public GameObject owner;
	public GameObject spawnedUnit;
	public float time;

	public SpawnUnit(GameObject owner, GameObject unit, float time) {
		this.owner = owner;
		this.spawnedUnit = unit;
		this.time = time;
	}

	public void Clear() {
		this.owner = null;
		this.spawnedUnit = null;
		this.time = 0f;
	}
};

public class Divisible : MonoBehaviour {
	private Selectable ownerSelectable;
	private Selectable spawnedSelectable;
	private NetworkView playerNetworkView;
	private float cooldownTimer = 15f;
	private bool isReady;
	private SpawnUnit spawnedUnit;
	private Vector3 spawnedLocation = Vector3.zero;
	private Vector3 rotatedVector = Vector3.zero;
	private float elapsedTime;


	public void Start() {
		this.ownerSelectable = this.GetComponent<Selectable>();
		this.isReady = true;
		this.playerNetworkView = this.GetComponent<NetworkView>();
		this.elapsedTime = 0f;
	}

	public void Update() {
		if (!this.isReady) {
			if (this.elapsedTime < 1f) {
				this.StartCoroutine(CR_MoveToPosition(this.gameObject, this.spawnedLocation + rotatedVector));
				this.StartCoroutine(CR_MoveToPosition(this.spawnedUnit.spawnedUnit, this.spawnedLocation - rotatedVector));
				this.StartCoroutine(CR_CooldownTime());
				this.elapsedTime += Time.deltaTime / this.cooldownTimer;
			}
		}
	}

	public void OnGUI() {
		if (this.ownerSelectable != null && this.playerNetworkView.isMine) {
			if (Input.GetKeyDown(KeyCode.S) && this.ownerSelectable.isSelected && this.isReady) {
				this.spawnedLocation = this.gameObject.transform.position;
				GameObject unit = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.spawnedLocation, Quaternion.identity, 0);
				unit.name = unit.name + " (Spawned)";

				this.spawnedSelectable = unit.GetComponentInChildren<Selectable>();
				this.spawnedSelectable.DisableSelection();
				this.ownerSelectable.DisableSelection();

				this.rotatedVector = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f) * (Vector3.right / 2f);

				NetworkView view = unit.GetComponent<NetworkView>();
				if (this.playerNetworkView != null && view != null) {
					this.playerNetworkView.RPC("RPC_Add", RPCMode.AllBuffered, this.playerNetworkView.viewID, view.viewID);
					this.playerNetworkView.RPC("RPC_Other_Spawn", RPCMode.OthersBuffered, this.spawnedLocation, this.rotatedVector);
				}
			}
		}
	}

	[RPC]
	private void RPC_Add(NetworkViewID first, NetworkViewID second) {
		this.isReady = false;
		this.elapsedTime = 0f;

		NetworkView firstView = NetworkView.Find(first);
		NetworkView secondView = NetworkView.Find(second);
		this.spawnedUnit = new SpawnUnit(firstView.gameObject, secondView.gameObject, this.cooldownTimer);
	}

	[RPC]
	private void RPC_Other_Spawn(Vector3 spawn, Vector3 rotated) {
		this.spawnedLocation = spawn;
		this.rotatedVector = rotated;
	}

	private IEnumerator CR_MoveToPosition(GameObject gameObject, Vector3 target) {
		while (this.elapsedTime < 1f) {
			gameObject.transform.position = Vector3.Lerp(this.spawnedLocation, target, this.elapsedTime);
			yield return null;
		}
	}

	public IEnumerator CR_CooldownTime() {
		Renderer rendererA = this.spawnedUnit.owner.GetComponentInChildren<Renderer>();
		Renderer rendererB = this.spawnedUnit.spawnedUnit.GetComponentInChildren<Renderer>();
		while (this.elapsedTime < 1f) {
			bool halfTime = this.elapsedTime < 0.5f;
			if (halfTime) {
				rendererA.material.color = Color.Lerp(Color.white, Color.cyan, this.elapsedTime);
				rendererB.material.color = Color.Lerp(Color.white, Color.cyan, this.elapsedTime);
			}
			else {
				rendererB.material.color = Color.Lerp(Color.cyan, Color.white, this.elapsedTime);
				rendererA.material.color = Color.Lerp(Color.cyan, Color.white, this.elapsedTime);
			}
			yield return null;
		}

		if (this.elapsedTime >= 1f) {
			if (this.ownerSelectable != null && !ownerSelectable.IsSelectionEnabled()) {
				this.ownerSelectable.EnableSelection();
			}
			else {
				Selectable owner = this.spawnedUnit.owner.GetComponent<Selectable>();
				if (owner != null && !owner.IsSelectionEnabled()) {
					owner.EnableSelection();
				}
			}
			if (this.spawnedSelectable != null && !this.spawnedSelectable.IsSelectionEnabled()) {
				this.spawnedSelectable.EnableSelection();
			}
			else {
				Selectable spawned = this.spawnedUnit.spawnedUnit.GetComponent<Selectable>();
				if (spawned != null && !spawned.IsSelectionEnabled()) {
					spawned.EnableSelection();
				}
			}

			this.isReady = true;
			rendererA.material.color = Color.white;
			rendererB.material.color = Color.white;
		}
	}
}