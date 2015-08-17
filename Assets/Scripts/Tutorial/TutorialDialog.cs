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
			switch (this.propertyIterator) {
				default:
					break;
				case 6:
				case 9:
				case 11:
				case 15:
					TutorialManager.Instance.IncrementState();
					break;
			}
		}

		private void Initialize() {
			Vector2 center = new Vector2((Screen.width - 300f) / 2f, -(Screen.height + 95f) / 2f);
			Vector2 upperLeftCorner = new Vector2(0f, -95f);
			DialogArrow empty = new DialogArrow(0f, 0f, 0f);
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Hello! Let's get started on how to play this game."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(83.82f, -259.75f, "This is your unit."), new DialogArrow(-193.24f, -91.92f, 110.5f), true, true, 0f));
			this.properties.Add(new DialogProperty(new DialogText(83.82f, -259.75f, "There are no other units."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(400f, -259.75f, "This is your opponent's unit."), new DialogArrow(174.92f, 101.9f, 287.9f), true, true, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "All units behave the same, regardless of faction."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "First up is unit selection."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "To select your unit, left click on the unit."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "You can also select your unit by dragging a box."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Next is camera panning."), new DialogArrow(0f, 0f, 0f), true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "To pan the camera around, move the mouse cursor near the edge."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Next, we make the unit move."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "Select the unit, and right click on the ground to move."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Next up is unit abilities."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "There are three abilities: Attack, Split, and Merge."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Let's start with Attacking."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "First, select your unit."), new DialogArrow(-193.24f, -91.92f, 110.5f), true, true, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "Next, press the A key to make it yellow."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Once your unit becomes yellow, right click near your opponent's unit to attack."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "When a unit is attacked, it will flash red briefly."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "When a unit's health is zero, it will disappear."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "That is how you wipe out your opponent and win."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Now, let's work on Splitting."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "To split, select your unit and press the S key."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "Your unit will then clone itself. Try and see!"), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "That is how you get a large army."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Finally, we try Merging."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "Select 2 units by box selecting them."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(upperLeftCorner.x, upperLeftCorner.y, "When you have 2 units selected, press the D key."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "As you merge, your unit grows larger."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "The larger the unit, the stronger its attack power, and the more health points (HP) it has."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "However, you cannot split a merged unit."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "Finally, the last lesson is Winning and Losing."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "To win, you must eradicate your opponent to defeat them."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "To lose, you must get wiped out by your opponent."), empty, true, false, 0f));
			this.properties.Add(new DialogProperty(new DialogText(center.x, center.y, "That is all I will teach you. Enjoy!"), empty, true, false, 0f));
		}
	}
}




