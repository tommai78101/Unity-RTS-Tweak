using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * TODO: Kill it slowly.
 * 
 */

public class DeathCheck : MonoBehaviour {
	public bool isDead;
	public static List<BattlePair> pairs = new List<BattlePair>();
	private NetworkView playerNetworkView;

	public void Awake() {
		this.isDead = false;
	}

	public void Start() {
		this.playerNetworkView = this.GetComponent<NetworkView>();
		if (this.playerNetworkView == null) {
			Debug.LogException(new System.NullReferenceException("There's no network view associated."));
		}
	}

	public void Kill() {
		this.isDead = true;
	}

	public void Update() {
		this.StartCoroutine(CR_Death());
	}

	public IEnumerator CR_Death() {
		for (int i = 0; i < DeathCheck.pairs.Count - 1; i++) {
			BattlePair a = DeathCheck.pairs[i];
			if (!a.isIgnored) {
				for (int j = i + 1; j < DeathCheck.pairs.Count; j++) {
					BattlePair b = DeathCheck.pairs[j];
					if (!b.isIgnored) {
						if (a.attackee.Equals(b.attacker) || b.attackee.Equals(a.attacker)) {
							b.SetIgnored();
						}
					}
				}
			}
		}
		for (int i = 0; i < DeathCheck.pairs.Count; i++) {
			BattlePair p = DeathCheck.pairs[i];
			if (!p.isIgnored) {
				DeathCheck check = p.attackee.GetComponent<DeathCheck>();
				if (check.isDead) {
					this.playerNetworkView = this.GetComponent<NetworkView>();
					if (this.playerNetworkView != null) {
						HealthBar hp = p.attackee.GetComponent<HealthBar>();
						if (hp != null) {
							hp.DecreaseHealth(1);
						}

						if (this.playerNetworkView.isMine && (hp.currentHealth <= 0 || HealthBar.GetHealthPercentage(p.attackee) <= 0f)) {
							NetworkPlayer[] players = Network.connections;
							if (players.Length > 0 && p.attackee != null) {
								Network.Destroy(p.attackee);
							}
							else if (p.attackee != null) {
								Object.Destroy(p.attackee);
							}
							DeathCheck.pairs.Remove(p);
							yield break;
						}
					}
				}
			}
			yield return null;
		}
	}
}
