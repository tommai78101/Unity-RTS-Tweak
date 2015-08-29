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

		void Update() {
			//Lesson 1 - Selecting (Box Selecting)
			//Lesson 2 - Camera Panning
			//Lesson 3 - Moving
			//Lesson 4 - Attacking
			//Lesson 4 - Splitting
			//Lesson 4 - Merging
			//Lesson 5 - Winning/Losing
			switch (this.stateCounter) {
				default:
				case 0:
					break;
				case 1:
					inputManager.selectionTutorialFlag = true;
					selectionManager.selectionTutorialFlag = true;
					break;
				case 2:
					inputManager.selectionTutorialFlag = true;
					selectionManager.selectionTutorialFlag = true;
					mainCamera.EnableTutorialFlag();
					break;
				case 3:
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					selectionManager.selectionTutorialFlag = true;
					mainCamera.EnableTutorialFlag();
					break;
				case 4:
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					inputManager.attackOrderTutorialFlag = true;
					selectionManager.selectionTutorialFlag = true;
					mainCamera.EnableTutorialFlag();
					break;
				case 5:
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					inputManager.attackOrderTutorialFlag = true;
					inputManager.splitTutorialFlag = true;
					selectionManager.selectionTutorialFlag = true;
					mainCamera.EnableTutorialFlag();
					break;
				case 6:
					inputManager.selectionTutorialFlag = true;
					inputManager.moveOrderTutorialFlag = true;
					inputManager.attackOrderTutorialFlag = true;
					inputManager.splitTutorialFlag = true;
					inputManager.mergeTutorialFlag = true;
					selectionManager.selectionTutorialFlag = true;
					mainCamera.EnableTutorialFlag();
					break;
			}
		}

		public void IncrementState() {
			this.stateCounter++;
		}
	}
}
