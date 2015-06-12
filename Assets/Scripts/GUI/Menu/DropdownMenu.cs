using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropdownMenu : MonoBehaviour {
	private Resolution[] resolutions;

	[SerializeField]
	private Transform menuPanel;

	[SerializeField]
	GameObject buttonPrefab;
	
	// Use this for initialization
	void Start() {
		this.resolutions = Screen.resolutions;
		for (int i = 0; i < this.resolutions.Length; i++) {
			GameObject button = (GameObject) Object.Instantiate(this.buttonPrefab);
			button.GetComponentInChildren<Text>().text = ResolutionToString(this.resolutions[i]);
			button.transform.parent = menuPanel;
			int index = i;
			button.GetComponent<Button>().onClick.AddListener(() => SetResolution(index));
		}
	}

	private string ResolutionToString(Resolution res) {
		return res.width + " x " + res.height;
	}

	private void SetResolution(int index) {
		Screen.SetResolution(this.resolutions[index].width, this.resolutions[index].height, false);
	}
}
