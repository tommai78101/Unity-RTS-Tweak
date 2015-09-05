using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace Tutorial {
	public class TutorialAttackManager : CommonAttackManager {
		protected void Start() {
			this.OnStartLocalPlayer();
		}

		public override void OnStartLocalPlayer() {
			base.OnStartLocalPlayer();
		}

		protected new void Update() {
			base.Update();
		}
	}
}
