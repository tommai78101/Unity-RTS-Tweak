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
	public bool confirmedDestroyed;

	public MergePair(GameObject a, GameObject b) {
		this.first = a;
		this.second = b;
		this.elapsedTime = 0f;
		this.firstInitialPosition = a.transform.position;
		this.secondInitialPosition = b.transform.position;
		this.firstInitialScale = a.transform.localScale;
		this.secondInitialScale = b.transform.localScale;
		this.confirmedDestroyed = false;

		HealthBar health = a.GetComponent<HealthBar>();
		if (health != null) {
			health.currentHealth *= 2;
			health.maxHealth *= 2;
			health.healthPercentage = (float) health.currentHealth / (float) health.maxHealth;
		}

		health = b.GetComponent<HealthBar>();
		if (health != null) {
			health.currentHealth *= 2;
			health.maxHealth *= 2;
			health.healthPercentage = (float) health.currentHealth / (float) health.maxHealth;
		}

		Attackable attack = a.GetComponent<Attackable>();
		if (attack != null) {
			attack.attackPower *= 2;
		}
		attack = b.GetComponent<Attackable>();
		if (attack != null) {
			attack.attackPower *= 2;
		}

		Level level = a.GetComponent<Level>();
		if (level != null) {
			level.IncrementLevel();
		}
		level = b.GetComponent<Level>();
		if (level != null) {
			level.IncrementLevel();
		}

		Selectable select = a.GetComponent<Selectable>();
		if (select != null) {
			select.Deselect();
		}
		select = b.GetComponent<Selectable>();
		if (select != null) {
			select.Deselect();
		}

		//Mergeable merge = a.GetComponent<Mergeable>();
		//if (merge != null) {
		//	merge.mergeLevel++;
		//}
		//merge = b.GetComponent<Mergeable>();
		//if (merge != null) {
		//	merge.mergeLevel++;
		//}
	}
};

public class MergeManager : MonoBehaviour {
	[SerializeField] public List<MergePair> pairs;
	[SerializeField] public List<MergePair> pendingToRemove;
	public float mergeCooldown = 3f;

	void Start() {
		this.pairs = new List<MergePair>();
		this.pendingToRemove = new List<MergePair>();
		this.pairs.Clear();
	}

	public void Update() {
		if (this.pairs.Count > 0) {
			for (int i = 0; i < this.pairs.Count; i++) {
				if (this.pairs[i].elapsedTime < 1f) {
					MoveTo(i);
					Scale(i, 0f, 1f);
					MergeUpdate(i);
				}
				else {
					MergePair pair = this.pairs[i];
					if (pair.second != null) {
						Network.Destroy(pair.second);
					}
					if (pair.first != null) {
						Selectable select = pair.first.GetComponent<Selectable>();
						if (select) {
							select.EnableSelection();
							select.Deselect();
						}
						Divisible div = pair.first.GetComponent<Divisible>();
						if (div != null) {
							div.SetDivisible(false);
						}
					}

					if (pair.first == null || pair.second == null || pair.confirmedDestroyed) {
						this.pendingToRemove.Add(pair);
						break;
					}
				}
			}
		}
		if (this.pendingToRemove.Count > 0) {
			foreach (MergePair pair in this.pendingToRemove) {
				if (this.pairs.Contains(pair)) {
					Selectable select = pair.first.GetComponent<Selectable>();
					if (select != null) {
						select.EnableSelection();
						select.Deselect();
					}
					this.pairs.Remove(pair);
				}
			}
			this.pendingToRemove.Clear();
		}
	}

	private void Scale(int i, float multiplierFrom, float multiplierTo) {
		MergePair pair = this.pairs[i];
		if (pair.first != null && pair.second != null) {
			float value = Mathf.Lerp(multiplierFrom, multiplierTo, pair.elapsedTime);
			pair.first.transform.localScale = pair.firstInitialScale + new Vector3(value, value, value);
			pair.second.transform.localScale = pair.secondInitialScale + new Vector3(value, value, value);
			this.pairs[i] = pair;
		}
	}

	private void MoveTo(int i) {
		MergePair pair = this.pairs[i];
		if (pair.first != null && pair.second != null) {
			pair.first.transform.position = Vector3.Lerp(pair.firstInitialPosition, pair.average, pair.elapsedTime);
			pair.second.transform.position = Vector3.Lerp(pair.secondInitialPosition, pair.average, pair.elapsedTime);
			this.pairs[i] = pair;
		}
	}

	private void MergeUpdate(int i) {
		MergePair pair = this.pairs[i];
		pair.elapsedTime += Time.deltaTime / this.mergeCooldown;
		this.pairs[i] = pair;
	}
}
