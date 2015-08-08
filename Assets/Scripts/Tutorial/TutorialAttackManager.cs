using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public struct AttackOrder {
		public Vector3 target;
		public List<GameObject> attackingUnits;

		public void Create(Vector3 target, List<GameObject> list) {
			this.target = target;
			if (this.attackingUnits == null) {
				this.attackingUnits = new List<GameObject>();
			}
			this.attackingUnits.AddRange(list);
		}
	};

	public class TutorialAttackManager : MonoBehaviour {
		public List<AttackOrder> attackOrders;
		public List<AttackOrder> destinationSet;

		// Use this for initialization
		void Start() {
			this.attackOrders = new List<AttackOrder>();
			this.destinationSet = new List<AttackOrder>();
		}

		// Update is called once per frame
		void Update() {
			if (this.attackOrders.Count > 0) {
				foreach (AttackOrder order in this.attackOrders) {
					foreach (GameObject obj in order.attackingUnits) {
						NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
						agent.SetDestination(order.target);
						this.destinationSet.Add(order);
					}
				}
			}
			if (this.destinationSet.Count > 0) {
				foreach (AttackOrder order in this.destinationSet) {
					if (this.attackOrders.Contains(order)) {
						this.attackOrders.Remove(order);
					}
				}
			}
		}
	}
}
