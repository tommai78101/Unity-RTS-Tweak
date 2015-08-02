using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Divisible : MonoBehaviour {
	private Selectable ownerSelectable;
	private Selectable spawnedSelectable;
	private Attackable ownerAttackable;
	private NetworkView playerNetworkView;
	private bool isReady;
	private bool canDivide;
	 
	public int numberOfUnitsPerSpawn = 1;
	public float cooldownTimer = 15f;
	public GameObject spawnUnitPrefab;
	public SplitManager splitManager;

	public void Start() {
		this.ownerSelectable = this.GetComponent<Selectable>();
		this.SetDivisibleReady();
		this.playerNetworkView = this.GetComponent<NetworkView>();
		this.ownerAttackable = this.GetComponent<Attackable>();
		this.canDivide = true;

		if (this.spawnUnitPrefab == null) {
			Debug.LogError(new System.NullReferenceException("Spawn Unit Prefab in Divisible is currently null. Please check  the owning unit's Divisble component."));
		}

		this.splitManager = GameObject.Find("Split Manager").GetComponent<SplitManager>();
		if (this.splitManager == null) {
			Debug.LogError("Split Manager is wrong here.");
		}
	}

	public void OnGUI() {
		if (this.playerNetworkView.isMine) {
			if (Input.GetKeyDown(KeyCode.S) && this.ownerSelectable.isSelected && this.isReady && (!this.ownerAttackable.isReadyToAttack || !this.ownerAttackable.isAttacking) && this.canDivide) {

				for (int j = 0; j < Selectable.selectedObjects.Count; j++) {
					Selectable ownerSelectable = Selectable.selectedObjects[j];
					if (ownerSelectable == null) {
						continue;
					}
					ownerSelectable.Deselect();
					Divisible ownerDivisible = ownerSelectable.gameObject.GetComponent<Divisible>();
					ownerDivisible.SetDivisibleNotReady();

					for (int i = 0; i < this.numberOfUnitsPerSpawn; i++) {
						Vector3 spawnedLocation = ownerSelectable.gameObject.transform.position;
						GameObject unit = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), spawnedLocation, Quaternion.identity, 0);
						//Clones will not have parentheses around the remote node label (client, or server).
						unit.name = (Network.isClient ? "client " : "server ") + System.Guid.NewGuid();

						HealthBar foo = unit.GetComponent<HealthBar>();
						HealthBar bar = this.gameObject.GetComponent<HealthBar>();
						if (foo != null && bar != null) {
							foo.currentHealth = bar.currentHealth;
							foo.maxHealth = bar.maxHealth;
							foo.healthPercentage = bar.healthPercentage;
						}

						Selectable spawnedSelectable = unit.GetComponentInChildren<Selectable>();
						spawnedSelectable.Deselect();
						ownerSelectable.Deselect();

						if (!Debugging.debugEnabled) {
							NetworkView view = unit.GetComponent<NetworkView>();
							if (this.playerNetworkView != null && view != null) {
								float randomValue = Random.Range(-180f, 180f);
								this.playerNetworkView.RPC("RPC_Add", RPCMode.AllBuffered, ownerSelectable.GetComponent<NetworkView>().viewID, view.viewID, randomValue);
							}
						}
						else {
							Debug.LogWarning("Debug flag is enabled.");
						}
					}
				}
			}
		}
	}

	[RPC]
	private void RPC_Add(NetworkViewID first, NetworkViewID second, float randomValue) {
		NetworkView firstView = NetworkView.Find(first);
		NetworkView secondView = NetworkView.Find(second);

		Divisible div = firstView.gameObject.GetComponent<Divisible>();
		div.SetDivisibleNotReady();

		Divisible div2 = secondView.gameObject.GetComponent<Divisible>();
		div.SetDivisibleNotReady();

		HealthBar foo = firstView.gameObject.GetComponent<HealthBar>();
		HealthBar bar = secondView.gameObject.GetComponent<HealthBar>();
		foo.Copy(bar);

		//Debug.Log((Network.isClient ? "(client)" : "(server)") + " is now making SpawnUnit struct object.");
		this.splitManager.spawnGroups.Add(new SpawnGroup(firstView.gameObject, secondView.gameObject, this.cooldownTimer, randomValue));
	}

	public void SetDivisible(bool flag) {
		this.canDivide = flag;
	}

	public bool IsDivisible() {
		return this.canDivide;
	}

	public bool IsDivisibleStateReady() {
		return this.isReady;
	}

	public void SetDivisibleReady() {
		this.isReady = true;
	}

	public void SetDivisibleNotReady() {
		this.isReady = false;
	}
}
