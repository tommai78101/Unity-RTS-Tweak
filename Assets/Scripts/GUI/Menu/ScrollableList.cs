using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollableList : MonoBehaviour {
	public GameObject itemPrefab;
	private HostData[] HostDataArray;
	private float RefreshRequestDuration = 3f;

	public void Generate() {
		RectTransform itemPrefabTransform = itemPrefab.GetComponent<RectTransform>();
		RectTransform containerTransform = this.gameObject.GetComponent<RectTransform>();

		float width = containerTransform.rect.width;
		float ratio = width / itemPrefabTransform.rect.width;
		float height = itemPrefabTransform.rect.height * ratio;

		float scrollHeight = height * (float) this.HostDataArray.Length;
		containerTransform.offsetMin = new Vector2(containerTransform.offsetMin.x, -scrollHeight / 2f);
		containerTransform.offsetMax = new Vector2(containerTransform.offsetMax.x, scrollHeight / 2f);

		for (int i = 0; i < this.HostDataArray.Length; i++) {
			GameObject newItem = (GameObject) Object.Instantiate<GameObject>(itemPrefab);
			newItem.name = newItem.name + " " + i.ToString();
			ButtonProperties exampleButton = newItem.GetComponent<ButtonProperties>();
			exampleButton.buttonName.text = this.HostDataArray[i].gameName;
			newItem.transform.SetParent(this.gameObject.transform, false);

			//RectTransform newItemTransform = newItem.GetComponent<RectTransform>();
			//float x = -containerTransform.rect.width / 2f;
			//float y = containerTransform.rect.height / 2f - 30f;
			//newItemTransform.offsetMin = new Vector2(x, y);
			
			//x = newItemTransform.offsetMin.x + containerTransform.rect.width - (containerTransform.rect.width / 10f);
			//y = newItemTransform.offsetMin.y + 30f;
			//newItemTransform.offsetMax = new Vector2(x, y);

			ServerEntryActions actions = newItem.GetComponent<ServerEntryActions>();
			if (actions != null){
				actions.data = this.HostDataArray[i];
			}
		}
	}

	public void FindServer() {
		Debug.Log("Finding servers.");
		this.StartCoroutine(this.CR_RefreshHostList());
	}

	private IEnumerator CR_RefreshHostList() {
		if (this.HostDataArray != null && this.HostDataArray.Length > 0) {
			System.Array.Clear(this.HostDataArray, 0, this.HostDataArray.Length);
		}
		MasterServer.RequestHostList(ButtonActions.RegisteredHostName);

		float StartTime = Time.time;
		float EndTime = StartTime + RefreshRequestDuration;
		while (Time.time < EndTime) {
			HostDataArray = MasterServer.PollHostList();
			yield return new WaitForEndOfFrame();
		}

		if (HostDataArray != null && HostDataArray.Length != 0) {
			if (HostDataArray.Length > 1) {
				Debug.Log(HostDataArray.Length + " have been found.");
			}
			else {
				Debug.Log(HostDataArray.Length + " has been found.");
			}
			Generate();
		}
		else {
			Debug.Log("Cannot find any servers.");
		}
	}
}
