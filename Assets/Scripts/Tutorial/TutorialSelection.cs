using UnityEngine;
using System.Collections;

namespace Tutorial {
	public class TutorialSelection : Selection {
		public static TutorialSelection Instance;
		public bool selectionTutorialFlag;

		public void Awake() {
			TutorialSelection.Instance = this;
		}

		public override void Update() {
			if (!this.selectionTutorialFlag) {
				return;
			}
			base.Update();
		}
	}
}
