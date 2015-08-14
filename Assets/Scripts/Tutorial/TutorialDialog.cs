using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	[Serializable]
	public struct DialogArrow {
		public Vector2 position;
		public float angle;

		public DialogArrow(float x, float y, float angle) {
			this.position = new Vector2(x, y);
			this.angle = angle;
		}
	}

	[Serializable]
	public struct DialogText {
		public Vector2 position;
		public string message;

		public DialogText(float x, float y, string message) {
			this.position = new Vector2(x, y);
			this.message = message;
		}
	}


	[Serializable]
	public struct DialogProperty {
		public DialogText dialogText;
		public DialogArrow dialogArrow;
		public bool changePosition;
		public bool showArrow;
		public float elapsedTime;

		public DialogProperty(DialogText text, DialogArrow arrow, bool shouldChangePositionOnNextDialog, bool showArrowOnNextDialog, float time) {
			this.dialogText = text;
			this.dialogArrow = arrow;
			this.changePosition = shouldChangePositionOnNextDialog;
			this.showArrow = showArrowOnNextDialog;
			this.elapsedTime = time;
		}
	}

	public class TutorialDialog : MonoBehaviour {
		public RectTransform dialogTransform;
		public RectTransform arrowTransform;
		public GameObject mainDialog;
		public GameObject arrow;
		public List<DialogProperty> properties;
		public int propertyIterator;
		public bool runCoroutine;

		void Start() {
			if (this.mainDialog == null) {
				Debug.LogError("Dialog: Something is wrong.");
			}
			if (this.arrow == null) {
				Debug.LogError("Arrow: Something is wrong.");
			}
			this.dialogTransform = this.mainDialog.GetComponent<RectTransform>();
			this.arrowTransform = this.arrow.GetComponent<RectTransform>();
			this.properties = new List<DialogProperty>();
			this.runCoroutine = false;
			Initialize();
		}

		void OnGUI() {
			if (this.propertyIterator < this.properties.Count) {
				DialogProperty property = this.properties[this.propertyIterator];
				Text text = this.mainDialog.GetComponentInChildren<Text>();
				if (text != null) {
					text.text = property.dialogText.message;
				}
				if (property.changePosition) {
					if (this.dialogTransform != null) {
						this.dialogTransform.anchoredPosition = property.dialogText.position;
					}
				}
				if (property.showArrow) {
					this.arrowTransform.anchoredPosition = property.dialogArrow.position;
					this.arrowTransform.localRotation = Quaternion.Euler(0f, 0f, property.dialogArrow.angle);
				}
				else {
					this.arrowTransform.anchoredPosition = new Vector2(0f, 260f);
				}
			}
		}

		void Update() {
			if (this.runCoroutine) {
				this.StartCoroutine(CR_DialogSwitch());
			}
		}

		private IEnumerator CR_DialogSwitch() {
			DialogProperty property = this.properties[this.propertyIterator];
			if (property.elapsedTime > 0f) {
				if (this.mainDialog.activeSelf) {
					this.mainDialog.SetActive(false);
				}
				if (this.arrow.activeSelf) {
					this.arrow.SetActive(false);
				}
				this.properties[this.propertyIterator] = property;
				yield return new WaitForSeconds(property.elapsedTime);
			}
			if (!this.mainDialog.activeSelf) {
				this.mainDialog.SetActive(true);
			}
			if (!this.arrow.activeSelf) {
				this.arrow.SetActive(true);
			}
			this.runCoroutine = false;
		}

		public void OnTutorialButtonClick() {
			this.propertyIterator++;
			if (this.propertyIterator < this.properties.Count) {
				this.runCoroutine = true;
			}
		}

		private void Initialize() {
			this.properties.Add(new DialogProperty(new DialogText((Screen.width - 300f) / 2f, -(Screen.height + 95f) / 2f, "Hello! Let's get started on how to play this game."), new DialogArrow(0f, 0f, 135f), true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(83.82f, -140.72f, "This is your unit."), new DialogArrow(218f, -313.3f, 135f), true, true, 0f));
			this.properties.Add(new DialogProperty(new DialogText(83.82f, -140.72f, "There are no other units."), new DialogArrow(218f, -313.3f, 135f), true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(400f, -140.72f, "This is your opponent's unit."), new DialogArrow(631.4f, -84.2f, 135f), true, true, 0f));
		}
	}
}

