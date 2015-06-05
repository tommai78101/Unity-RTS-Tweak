using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MergePair {
	public GameObject first;
	public GameObject second;
	public float elapsedTime;
	public Vector3 firstInitialPosition;
	public Vector3 secondInitialPosition;
	public Vector3 firstInitialScale;
	public Vector3 secondInitialScale;
	public Vector3 average {
		get {
			return (this.firstInitialPosition + ((this.secondInitialPosition - this.firstInitialPosition) / 2f));
		}
	}

	public MergePair(GameObject a, GameObject b) {
		this.first = a;
		this.second = b;
		this.elapsedTime = 0f;
		this.firstInitialPosition = a.transform.position;
		this.secondInitialPosition = b.transform.position;
		this.firstInitialScale = a.transform.localScale;
		this.secondInitialScale = b.transform.localScale;
	}
};

public class Mergeable : MonoBehaviour {
	private Selectable ownerSelectable;
	private static List<MergePair> pairs = new List<MergePair>();
	private NetworkView playerNetworkView;

	[RPC]
	public void RPC_AddPair(NetworkViewID firstViewID, NetworkViewID secondViewID) {
		NetworkView first = NetworkView.Find(firstViewID);
		NetworkView second = NetworkView.Find(secondViewID);
		pairs.Add(new MergePair(first.gameObject, second.gameObject));
	}

	public void Start() {
		this.ownerSelectable = this.GetComponent<Selectable>();
		Mergeable.pairs.Clear();
		this.playerNetworkView = this.GetComponent<NetworkView>();
	}

	public void Update() {
		if (Mergeable.pairs.Count > 0) {
			this.StartCoroutine(CR_Action());
		}
	}

	public void OnGUI() {
		if (Input.GetKeyDown(KeyCode.D) && this.ownerSelectable.isSelected) {
			this.ownerSelectable.isSelected = false;
			this.ownerSelectable.DisableSelection();
			if (this.playerNetworkView != null) {
				int count = Selectable.selectedObjects.Count;
				while (count >= 2) {
					Selectable firstSelectable = Selectable.selectedObjects[0];
					Selectable secondSelectable = Selectable.selectedObjects[1];
					NetworkView firstNetworkView = firstSelectable.GetComponent<NetworkView>();
					NetworkView secondNetworkView = secondSelectable.GetComponent<NetworkView>();

					this.playerNetworkView.RPC("RPC_AddPair", RPCMode.AllBuffered, firstNetworkView.viewID, secondNetworkView.viewID);

					firstSelectable.DisableSelection();
					secondSelectable.DisableSelection();
					Selectable.selectedObjects.Remove(firstSelectable);
					Selectable.selectedObjects.Remove(secondSelectable);

					count -= 2;
				}
			}
		}
	}

	private IEnumerator CR_Action() {
		for (int i = 0; i < Mergeable.pairs.Count; i++) {
			MoveTo(i);
			Scale(i, 1f, 2f);
			Update(i);

			if (Mergeable.pairs[i].elapsedTime > 1f) {
				Selectable select = Mergeable.pairs[i].first.GetComponent<Selectable>();
				if (select != null) {
					select.EnableSelection();
				}
				Network.Destroy(Mergeable.pairs[i].second);
				Mergeable.pairs.Remove(Mergeable.pairs[i]);
			}
			yield return null;
		}
	}

	public void OnDestroy() {
		UnitManager.instance.PlayerUnits.Remove(this.gameObject);
		UnitManager.instance.AllUnits.Remove(this.gameObject);
	}

	private void Scale(int i, float multiplierFrom, float multiplierTo) {
		MergePair pair = Mergeable.pairs[i];
		if (pair.first != null && pair.second != null) {
			pair.first.transform.localScale = pair.firstInitialScale * Mathf.Lerp(multiplierFrom, multiplierTo, pair.elapsedTime);
			pair.second.transform.localScale = pair.secondInitialScale * Mathf.Lerp(multiplierFrom, multiplierTo, pair.elapsedTime);
			Mergeable.pairs[i] = pair;
		}
	}

	private void MoveTo(int i) {
		MergePair pair = Mergeable.pairs[i];
		if (pair.first != null && pair.second != null) {
			pair.first.transform.position = Vector3.Lerp(pair.firstInitialPosition, pair.average, pair.elapsedTime);
			pair.second.transform.position = Vector3.Lerp(pair.secondInitialPosition, pair.average, pair.elapsedTime);
			Mergeable.pairs[i] = pair;
		}
	}

	private void Update(int i) {
		MergePair pair = Mergeable.pairs[i];
		pair.elapsedTime += Time.deltaTime / 15f;
		Mergeable.pairs[i] = pair;
	}
}
