using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Common {
	[Serializable]
	public struct SplitGroup {
		public GameObject owner;
		public GameObject clone;
		public float elapsedTime;
		public Vector3 center;
		public Vector3 ownerTarget;
		public Vector3 cloneTarget;

		public SplitGroup(GameObject owner, GameObject clone) {
			this.owner = owner;
			this.clone = clone;
			this.elapsedTime = 0f;
			this.center = owner.transform.position;

			Renderer renderer = owner.GetComponent<Renderer>();
			Vector3 size = renderer.bounds.size;
			float angle = UnityEngine.Random.Range(-180f, 180f);
			Vector3 rotation = Quaternion.Euler(0f, angle, 0f) * (size / 2f);
			this.ownerTarget = new Vector3(this.center.x + rotation.x, this.center.y, this.center.z + rotation.z);
			this.cloneTarget = new Vector3(this.center.x - rotation.x, this.center.y, this.center.z - rotation.z);
		}
	}

	public class CommonSplitManager : NetworkBehaviour {
		[SerializeField]
		public List<SplitGroup> splitGroups;
		[SerializeField]
		public List<SplitGroup> removeList;

		protected void Start() {
			this.splitGroups = new List<SplitGroup>();
			this.removeList = new List<SplitGroup>();
		}

		protected void Update() {
			if (this.splitGroups.Count > 0) {
				for (int i = 0; i < this.splitGroups.Count; i++) {
					SplitGroup group = this.splitGroups[i];
					if (group.elapsedTime < 1f) {
						group.owner.transform.position = Vector3.Lerp(group.center, group.ownerTarget, group.elapsedTime);
						group.clone.transform.position = Vector3.Lerp(group.center, group.cloneTarget, group.elapsedTime);
						group.elapsedTime += Time.deltaTime;
						this.splitGroups[i] = group;
						//TODO(Thompson): Test to see if "ref" keyword works.
						//MovePosition(ref group);
						//UpdateTime(ref group);
					}
					else {
						if (!this.removeList.Contains(group)) {
							this.removeList.Add(group);
						}
					}
				}
			}
			if (this.removeList.Count > 0) {
				foreach (SplitGroup group in this.removeList) {
					if (this.splitGroups.Contains(group)) {
						CommonUnit unit = group.owner.GetComponent<CommonUnit>();
						unit.EnableSelection();
						unit.SetNotSplitting();
						unit = group.clone.GetComponent<CommonUnit>();
						unit.EnableSelection();
						unit.SetNotSplitting();
						this.splitGroups.Remove(group);
					}
				}
				this.removeList.Clear();
			}
		}

		public void MovePosition(ref SplitGroup group) {
			group.owner.transform.position = Vector3.Lerp(group.center, group.ownerTarget, group.elapsedTime);
			group.clone.transform.position = Vector3.Lerp(group.center, group.cloneTarget, group.elapsedTime);
		}

		public void UpdateTime(ref SplitGroup group) {
			group.elapsedTime += Time.deltaTime;
		}
	}
}
