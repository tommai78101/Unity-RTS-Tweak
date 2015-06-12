using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item {
	public string itemName;
}

public class CreateScrollList : MonoBehaviour {
	
	public GameObject sampleButtonPrefab;
	public List<Item> itemList;
	public Transform contentPanel;

	// Use this for initialization
	void Start() {
		PopulateList();
	}

	void PopulateList() {
		foreach (var item in itemList) {
			GameObject newButton = Object.Instantiate(sampleButtonPrefab) as GameObject;
			ButtonProperties exampleButton = newButton.GetComponent<ButtonProperties>();
			exampleButton.buttonName.text = item.itemName;
			newButton.transform.SetParent(this.contentPanel.transform);
		}
	}
}
