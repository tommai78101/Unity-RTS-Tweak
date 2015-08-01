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


	public void OnGUI() {
		if (Input.GetKeyDown(KeyCode.D) && this.ownerSelectable.isSelected && !(this.ownerAttackable.isReadyToAttack || this.ownerAttackable.isAttacking)) {
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

	//private IEnumerator CR_Action() {
	//	for (int i = 0; i < this.mergeManager.pairs.Count; i++) {
	//		MoveTo(i);
	//		Scale(i, 0f, 1f);
	//		Update(i);

	//		MergePair pair = this.mergeManager.pairs[i];
	//		if (pair.elapsedTime > 1f) {
	//			if (pair.first != null) {
	//				Selectable select = pair.first.GetComponent<Selectable>();
	//				if (select != null) {
	//					select.EnableSelection();
	//				}
	//			}

	//			bool isDestroyed = false;
	//			if (pair.second != null) {
	//				if (pair.second.activeSelf && this.playerNetworkView.isMine) {
	//					Network.RemoveRPCsInGroup(0);
	//					Network.Destroy(pair.second);
					
	//					Divisible div = pair.first.GetComponent<Divisible>();
	//					if (div != null && div.IsDivisible()) {
	//						div.SetDivisible(false);
	//					}
	//				}

	//				Mergeable secondMerge = pair.second.GetComponent<Mergeable>();
	//				if (secondMerge.confirmedDestroyed) {
	//					isDestroyed = true;
	//					pair.second = null;
	//				}
	//			}

	//			if (pair.first == null || isDestroyed) {
	//				this.mergeManager.pairs.Remove(pair);
	//			}
	//		}
	//		else {
	//			Selectable select = pair.first.GetComponent<Selectable>();
	//			if (select != null) {
	//				select.DisableSelection();
	//			}
	//			select = pair.second.GetComponent<Selectable>();
	//			if (select != null) {
	//				select.DisableSelection();
	//			}
	//		}
	//		yield return null;
	//	}
	//}

	//private void Scale(int i, float multiplierFrom, float multiplierTo) {
	//	MergePair pair = this.mergeManager.pairs[i];
	//	if (pair.first != null && pair.second != null) {
	//		float value = Mathf.Lerp(multiplierFrom, multiplierTo, pair.elapsedTime);
	//		pair.first.transform.localScale = pair.firstInitialScale + new Vector3(value, value, value);
	//		pair.second.transform.localScale = pair.secondInitialScale + new Vector3(value, value, value);
	//		this.mergeManager.pairs[i] = pair;
	//	}
	//}

	//private void MoveTo(int i) {
	//	MergePair pair = this.mergeManager.pairs[i];
	//	if (pair.first != null && pair.second != null) {
	//		pair.first.transform.position = Vector3.Lerp(pair.firstInitialPosition, pair.average, pair.elapsedTime);
	//		pair.second.transform.position = Vector3.Lerp(pair.secondInitialPosition, pair.average, pair.elapsedTime);
	//		this.mergeManager.pairs[i] = pair;
	//	}
	//}

	//private void Update(int i) {
	//	MergePair pair = this.mergeManager.pairs[i];
	//	pair.elapsedTime += Time.deltaTime / 15f;
	//	this.mergeManager.pairs[i] = pair;
	//}

	public void OnDestroy() {
		this.pairReference.confirmedDestroyed = true;
	}
}
