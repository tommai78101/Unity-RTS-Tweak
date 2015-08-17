using UnityEngine;
using System.Collections;

namespace Tutorial {
	public class TutorialManager : MonoBehaviour {
		public static TutorialManager Instance;

		public TutorialInputManager inputManager;
		public TutorialSelection selectionManager;
		public TutorialCameraPanning mainCamera;

		public int stateCounter;

		void Awake() {
			TutorialManager.Instance = this;
		}

		// Use this for initialization
		void Start() {
			this.stateCounter = 0;
			selectionManager.selectionTutorialFlag = false;
			mainCamera.cameraPanningTutorialFlag = false;
			inputManager.selectionTutorialFlag = false;
			inputManager.moveOrderTutorialFlag = false;
			inputManager.attackOrderTutorialFlag = false;
			inputManager.splitTutorialFlag = false;
			inputManager.mergeTutorialFlag = false;
		}

		void OnGUI() {
			switch (this.stateCounter) {
				default:
				case 0:
					break;
				case 1:
					selectionManager.selectionTutorialFlag = true;
					inputManager.selectionTutorialFlag = true;
					break;
				case 2:
					selectionManager.selectionTutorialFlag = true;
					inputManager.selectionTutorialFlag = true;
					mainCamera.cameraPanningTutorialFlag = true;
					break;
				case 3:
					selectionManager.selectionTutorialFlag = true;
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					mainCamera.cameraPanningTutorialFlag = true;
					break;
				case 4:
					selectionManager.selectionTutorialFlag = true;
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					inputManager.attackOrderTutorialFlag = true;
					mainCamera.cameraPanningTutorialFlag = true;
					break;
				case 5:
					selectionManager.selectionTutorialFlag = true;
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					inputManager.attackOrderTutorialFlag = true;
					inputManager.splitTutorialFlag = true;
					mainCamera.cameraPanningTutorialFlag = true;
					break;
				case 6:
					selectionManager.selectionTutorialFlag = true;
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					inputManager.attackOrderTutorialFlag = true;
					inputManager.splitTutorialFlag = true;
					inputManager.mergeTutorialFlag = true;
					mainCamera.cameraPanningTutorialFlag = true;
					break;
			}
		}

		public void IncrementState() {
			this.stateCounter++;
		}
	}
}
