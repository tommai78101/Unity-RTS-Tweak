using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace Tutorial {
	public class TutorialInputManager : CommonInputManager {
		public bool selectionTutorialFlag;
		public bool attackOrderTutorialFlag;
		public bool moveOrderTutorialFlag;
		public bool splitTutorialFlag;
		public bool mergeTutorialFlag;

		//----------------------------------

		protected new void Start() {
			base.Start();

			//this.selectionTutorialFlag = this.attackOrderTutorialFlag = this.moveOrderTutorialFlag = this.splitTutorialFlag = this.mergeTutorialFlag = true;

			if (this.attackManager == null) {
				Debug.LogError("Tutorial: Cannot find attack manager for the tutorial.");
			}
			if (this.splitManager == null) {
				Debug.LogError("Tutorial: Cannot find split manager for the tutorial.");
			}

			TutorialUnitManager.Instance.InitializeObjectList();
			GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("Tutorial_Unit");
			foreach (GameObject obj in existingObjects) {
				TutorialUnitManager.Instance.getAllObjects().Add(obj);
			}
		}

		protected override void Update() {
			if (!EventSystem.current.IsPointerOverGameObject()) {
				SelectOrder();
				MoveOrder();
				SplitOrder();
				MergeOrder();

				base.UpdateStatus();
			}
		}

		protected new void SelectOrder() {
			if (!this.selectionTutorialFlag) {
				return;
			}
			if (this.attackStandingByFlag) {
				return;
			}
			base.SelectOrder();
		}

		protected new void AttackOrder() {
			if (!this.attackOrderTutorialFlag) {
				return;
			}
			base.AttackOrder();
		}


		protected new void MoveOrder() {
			if (!this.moveOrderTutorialFlag) {
				return;
			}
			base.MoveOrder();
		}


		protected new void SplitOrder() {
			if (!this.splitTutorialFlag) {
				return;
			}
			base.SplitOrder();
		}

		protected new void MergeOrder() {
			if (!this.mergeTutorialFlag) {
				return;
			}
			base.MergeOrder();
		}
	}
}
