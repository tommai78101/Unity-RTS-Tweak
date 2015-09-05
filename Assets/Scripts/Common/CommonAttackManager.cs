using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Common {
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

	public class CommonAttackManager : NetworkBehaviour {
		public List<AttackOrder> attackOrders;
		public List<AttackOrder> destinationSet;

		public override void OnStartServer() {
			this.OnStartLocalPlayer();
		}

		public override void OnStartLocalPlayer() {
			this.Start();
		}

		protected void Start() {
			this.attackOrders = new List<AttackOrder>();
			this.destinationSet = new List<AttackOrder>();
		}

		protected void Update() {
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
