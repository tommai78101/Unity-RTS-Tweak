using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tutorial {
	[Serializable]
	public struct DialogProperty {
		public Vector2 position;
		public string textMessage;
		public bool changePosition;
		public float elapsedTime;

		public DialogProperty(float x, float y, string message, bool shouldChangePositionOnNextDialog, float time) {
			this.position = new Vector2(x, y);
			this.textMessage = message;
			this.changePosition = shouldChangePositionOnNextDialog;
			this.elapsedTime = time;
		}
	}

	public class TutorialDialog : MonoBehaviour {
		public RectTransform dialogTransform;
		public GameObject mainDialog;
		public List<DialogProperty> properties;
		public int propertyIterator;
		public bool runCoroutine;

		void Start() {
			if (this.mainDialog == null) {
				Debug.LogError("Dialog: Something is wrong.");
			}
			this.dialogTransform = this.mainDialog.GetComponent<RectTransform>();
			this.properties = new List<DialogProperty>();
			this.runCoroutine = false;
			Initialize();
		}

		void OnGUI() {
			if (this.propertyIterator < this.properties.Count) {
				DialogProperty property = this.properties[this.propertyIterator];
				Text text = this.mainDialog.GetComponentInChildren<Text>();
				if (text != null) {
					text.text = property.textMessage;
					if (property.changePosition) {
						RectTransform transform = this.GetComponent<RectTransform>();
						if (transform != null) {
							transform.anchoredPosition = property.position;
						}
					}
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
				this.properties[this.propertyIterator] = property;
				yield return new WaitForSeconds(property.elapsedTime);
			}
			if (!this.mainDialog.activeSelf) {
				this.mainDialog.SetActive(true);
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
			this.properties.Add(new DialogProperty((Screen.width - 300f)/2f, -(Screen.height - 95f) /2f, "Hello! Let's get started on how to play this game.", true, 0f));
		}
	}
}
