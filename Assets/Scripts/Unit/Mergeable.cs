using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mergeable : MonoBehaviour {
	private Selectable ownerSelectable;
	private Attackable ownerAttackable;
	private NetworkView playerNetworkView;

	public MergePair pairReference;
	public MergeManager mergeManager;

	public int mergeLevel;

	[RPC]
	public void RPC_AddPair(NetworkViewID firstViewID, NetworkViewID secondViewID) {
		NetworkView first = NetworkView.Find(firstViewID);
		NetworkView second = NetworkView.Find(secondViewID);
		this.pairReference = new MergePair(first.gameObject, second.gameObject);
		this.mergeManager.pairs.Add(this.pairReference);
	}

	public void Start() {
		this.mergeManager = GameObject.Find("Merge Manager").GetComponent<MergeManager>();
		if (this.mergeManager == null) {
			Debug.LogError("Merge Manager is wrong here.");
		}
		this.ownerSelectable = this.GetComponent<Selectable>();
		this.playerNetworkView = this.GetComponent<NetworkView>();
		this.ownerAttackable = this.GetComponent<Attackable>();
		//this.mergeLevel = 1;
	}

	public void Update() {
		this.MergeInput();
	}


	private void MergeInput() {
		if (Input.GetKeyDown(KeyCode.D) && this.ownerSelectable.isSelected && !(this.ownerAttackable.isReadyToAttack || this.ownerAttackable.isAttacking)) {
			Analytics.Instance.AddEvent("D key pressed to merge " + Selectable.selectedObjects.Count.ToString() + " units.");
			this.ownerSelectable.Deselect();
			//this.ownerSelectable.DisableSelection();
			if (this.playerNetworkView != null) {
				foreach (Selectable sel in Selectable.selectedObjects) {
					sel.Deselect();
					sel.isBoxSelected = false;
				}
				int count = Selectable.selectedObjects.Count;
				int countCheck = count;
				int index = 0;
				while (count >= 2 && countCheck > 0) {
					Selectable firstSelectable = Selectable.selectedObjects[index];
					Selectable secondSelectable = Selectable.selectedObjects[index+1];
					if (firstSelectable != null && secondSelectable != null) {
						Level levelFirst = firstSelectable.gameObject.GetComponent<Level>();
						Level levelSecond = secondSelectable.gameObject.GetComponent<Level>();
						if (levelFirst != null && levelSecond != null && (levelFirst.Compare(levelSecond) == Level.EQUALS)) {
							NetworkView firstNetworkView = firstSelectable.GetComponent<NetworkView>();
							NetworkView secondNetworkView = secondSelectable.GetComponent<NetworkView>();
							this.playerNetworkView.RPC("RPC_AddPair", RPCMode.AllBuffered, firstNetworkView.viewID, secondNetworkView.viewID);
							firstSelectable.DisableSelection();
							secondSelectable.DisableSelection();
							Selectable.selectedObjects.Remove(firstSelectable);
							Selectable.selectedObjects.Remove(secondSelectable);
						}
						else {
							Selectable.selectedObjects.Remove(firstSelectable);
							Selectable.selectedObjects.Add(firstSelectable);
							countCheck--;
							continue;
						}
					}
					else {
						index++;
						if (index > count) {
							break;
						}
						countCheck = count;
						continue;
					}
					count -= 2;
					countCheck = count;
				}
				Selectable.selectedObjects.Clear();
			}
		}
	}

	public void OnDestroy() {
		this.pairReference.confirmedDestroyed = true;
	}
}
