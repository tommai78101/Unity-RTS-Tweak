using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Common {
	public class CommonUnitManager : NetworkBehaviour {
		public static CommonUnitManager Instance;

		[SerializeField]
		protected List<GameObject> allObjects;
		[SerializeField]
		protected List<GameObject> removeList;

		protected void Awake() {
			CommonUnitManager.Instance = this;
		}

		protected void Start() {
			this.InitializeObjectList();
			this.InitializeRemoveList();
		}

		protected void Update() {
			if (this.removeList.Count > 0) {
				foreach (GameObject obj in this.removeList) {
					if (this.allObjects.Contains(obj)) {
						GameObject.Destroy(obj);
						this.allObjects.Remove(obj);
					}
				}
				this.removeList.Clear();
			}
		}

		public List<GameObject> getAllObjects() {
			this.InitializeObjectList();
			return this.allObjects;
		}

		public List<GameObject> getRemoveList() {
			this.InitializeRemoveList();
			return this.removeList;
		}

		public void InitializeObjectList() {
			if (this.allObjects == null) {
				this.allObjects = new List<GameObject>();
			}
		}

		public void InitializeRemoveList() {
			if (this.removeList == null) {
				this.removeList = new List<GameObject>();
			}
		}
	}
}
