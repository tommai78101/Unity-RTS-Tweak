using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	public class TutorialUnitManager : MonoBehaviour {
		public static TutorialUnitManager Instance;

		public List<GameObject> allObjects;

		// Use this for initialization
		void Start() {
			TutorialUnitManager.Instance = this;
			this.allObjects = new List<GameObject>();
		}
	}
}
