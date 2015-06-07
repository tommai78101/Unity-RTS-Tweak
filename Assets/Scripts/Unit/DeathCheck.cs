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

	public void Kill() {
		this.isDead = true;
	}

	public void Update() {
		this.StartCoroutine(CR_Death());
	}

	public IEnumerator CR_Death() {
		for (int i = 0; i < DeathCheck.pairs.Count; i++) {
			BattlePair p = DeathCheck.pairs[i];
			DeathCheck check = p.attackee.GetComponent<DeathCheck>();
			if (check.isDead) {
				this.playerNetworkView = this.GetComponent<NetworkView>();
				if (this.playerNetworkView != null) {
					Attackable attack = p.attacker.GetComponent<Attackable>();
					if (attack.GetStrength() <= 0) {
						attack.IncreaseStrength();
					}
					p.attackee.SetActive(false);
					if (!p.attackee.activeSelf && this.playerNetworkView.isMine) {
						Network.Destroy(p.attackee);
					}
				}
				DeathCheck.pairs.Remove(p);
			}
			yield return null;
		}
	}
}
