using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	[Serializable]
	public struct MergeGroup {
		public GameObject owner;
		public GameObject merger;
		public float elapsedTime;
		public Vector3 center;
		public Vector3 ownerOrigin;
		public Vector3 mergerOrigin;
		public Vector3 initialLocalScale;
		public Vector3 newLocalScale;

		public MergeGroup(GameObject owner, GameObject merger) {
			this.owner = owner;
			this.merger = merger;
			this.elapsedTime = 0f;

			this.ownerOrigin = owner.transform.position;
			this.mergerOrigin = merger.transform.position;
			this.initialLocalScale = owner.transform.localScale;
			this.newLocalScale = this.initialLocalScale * 1.4f;
			this.center = this.ownerOrigin + ((this.mergerOrigin - this.ownerOrigin) / 2f);

			TutorialUnit ownerUnit = owner.GetComponent<TutorialUnit>();
			TutorialUnit mergerUnit = merger.GetComponent<TutorialUnit>();
			ownerUnit.MultiplyAttributes(2);
			mergerUnit.MultiplyAttributes(2);
		}
	}

	public class TutorialMergeManager : MonoBehaviour {
		public List<MergeGroup> mergeGroups;
		public List<MergeGroup> removeList;

		// Use this for initialization
		void Start() {
			this.mergeGroups = new List<MergeGroup>();
		}

		// Update is called once per frame
		void Update() {
			if (this.mergeGroups.Count > 0) {
				for (int i = 0; i < this.mergeGroups.Count; i++) {
					MergeGroup group = this.mergeGroups[i];
					if (group.elapsedTime < 1f) {
						group.owner.transform.position = Vector3.Lerp(group.ownerOrigin, group.center, group.elapsedTime);
						group.merger.transform.position = Vector3.Lerp(group.mergerOrigin, group.center, group.elapsedTime);
						group.owner.transform.localScale = Vector3.Lerp(group.initialLocalScale, group.newLocalScale, group.elapsedTime);
						group.merger.transform.localScale = Vector3.Lerp(group.initialLocalScale, group.newLocalScale, group.elapsedTime);
						group.elapsedTime += Time.deltaTime;
						this.mergeGroups[i] = group;
					}
					else {
						this.removeList.Add(this.mergeGroups[i]);
					}
				}
			}
			if (this.removeList.Count > 0) {
				foreach (MergeGroup group in this.removeList) {
					if (this.mergeGroups.Contains(group)) {
						TutorialUnit unit = group.owner.GetComponent<TutorialUnit>();
						unit.EnableSelection();
						unit.SetNotMerging();
						if (TutorialUnitManager.Instance.allObjects.Contains(group.merger)) {
							TutorialUnitManager.Instance.allObjects.Remove(group.merger);
						}
						GameObject.Destroy(group.merger);
						this.mergeGroups.Remove(group);
					}
				}
				this.removeList.Clear();
			}
		}
	}
}
