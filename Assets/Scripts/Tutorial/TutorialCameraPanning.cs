using UnityEngine;
using System.Collections;

public class TutorialCameraPanning : CameraPanning {
	public static TutorialCameraPanning Instance;
	public bool cameraPanningTutorialFlag;

	public void Awake() {
		TutorialCameraPanning.Instance = this;
	}

	public void Start() {
		this.cameraPanningTutorialFlag = false;
	}

	public override void Update() {
		if (!this.cameraPanningTutorialFlag) {
			return;
		}
		base.Update();
	}

	public void EnableTutorialFlag() {
		this.cameraPanningTutorialFlag = true;
		this.mouseInFocus = true;
	}
}
