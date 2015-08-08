using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public class TutorialUnitManager : NetworkBehaviour {
		public static TutorialUnitManager Instance;

		public List<GameObject> allObjects;
		public List<GameObject> removeList;

		void Start() {
			TutorialUnitManager.Instance = this;
			this.allObjects = new List<GameObject>();
		}

		void Update() {
			if (this.removeList.Count > 0) {
				foreach (GameObject obj in this.removeList) {
					if (this.allObjects.Contains(obj)) {
						this.allObjects.Remove(obj);
					}
				}
				this.removeList.Clear();
			}
		}
	}
}
