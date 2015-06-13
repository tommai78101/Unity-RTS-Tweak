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
				Debug.Log("Battle pair is not ignored.");
				DeathCheck check = p.attackee.GetComponent<DeathCheck>();
				if (check.isDead) {
					Debug.Log("Battle pair " + i.ToString() + " has dead attackee.");
					this.playerNetworkView = this.GetComponent<NetworkView>();
					if (this.playerNetworkView != null) {
						Debug.Log("Network view is still active.");
						Attackable attack = p.attacker.GetComponent<Attackable>();
						if (attack.GetStrength() <= 0) {
							Debug.Log("Attacker won. Strength increased.");
							attack.IncreaseStrength();
						}
						if (this.playerNetworkView.isMine) {
							NetworkPlayer[] players = Network.connections;
							if (players.Length > 0 && p.attackee != null) {
								Debug.Log("Destroying attackee through network: " + p.attackee.name);
								Network.Destroy(p.attackee);
							}
							else if (p.attackee != null) {
								Debug.Log("Destroying attackee by normal means: " + p.attackee.name);
								Object.Destroy(p.attackee);
							}
							Debug.Log("Removing pair");
							DeathCheck.pairs.Remove(p);
							Debug.Log("Breaking away from coroutine.");
							yield break;
						}
					}
				}
			}
			Debug.Log("Temporary returning from coroutine.");
			yield return null;
		}
	}
}
