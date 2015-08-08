using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	[Serializable]
	public struct MergeGroup {
		public GameObject owner;
		public GameObject merger;

		public MergeGroup(GameObject owner, GameObject merger) {
			this.owner = owner;
			this.merger = merger;
		}
	}

	public class TutorialMergeManager : MonoBehaviour {
		public List<MergeGroup> mergeGroups;

		// Use this for initialization
		void Start() {
			this.mergeGroups = new List<MergeGroup>();
		}

		// Update is called once per frame
		void Update() {
			if (this.mergeGroups.Count > 0) {
				//TODO: Work more on this.
			}
		}
	}
}
