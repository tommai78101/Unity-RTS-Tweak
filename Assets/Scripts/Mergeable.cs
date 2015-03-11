using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mergeable : MonoBehaviour {
	public static bool mergeFlag = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.M)) {
			Mergeable.mergeFlag = true;
			Debug.Log("Merge Flag enabled.");
		}
		else {
			Mergeable.mergeFlag = false;
			Debug.Log("Merge Flag disabled.");
		}

		if (Mergeable.mergeFlag) {
			List<Selectable> objects = Selectable.selectedObjects;
			if (objects.Count == 2) {
				Debug.Log("Merge object count: " + objects.Count);
				Mergeable mergeObject = objects[objects.Count - 1].GetComponent<Mergeable>();
				if (mergeObject != null) {
					Merge(mergeObject);
					Destroy(Selectable.selectedObjects[0]);
					Destroy(objects[0]);
					for (int i = 0; i < objects.Count; i++){
						if (objects[i].transform.localScale != Vector3.one) {
							Division d = objects[i].GetComponent<Division>();
							d.SetCannotDivideFlag();
						}
						if (objects[i] == null) {
							Selectable.selectedObjects.RemoveAt(i);
							objects.RemoveAt(i);
						}
					}
					Mergeable.mergeFlag = false;
					Debug.Log("Merge Flag is now disabled: " + Mergeable.mergeFlag);
				}
			}
			Mergeable.mergeFlag = false;
			Debug.Log("Merge Flag disabled.");
		}
		else {
			Debug.Log("Selected objects count: " + Selectable.selectedObjects.Count + "  Capacity: " + Selectable.selectedObjects.Capacity);
		}
	}

	public void Merge(Mergeable mergeable) {
		Debug.Log("Merging objects.");
		GameObject mergedObject = Instantiate(mergeable.gameObject) as GameObject;
		mergedObject.transform.localScale += Vector3.one;
		mergedObject.transform.position = (this.transform.position + mergedObject.transform.position) / 2f;
		mergedObject.name = "Player " + Division.id++;
		Debug.Log("Destroying game objects.");
		Destroy(mergeable.gameObject);
		Destroy(this.gameObject);

		Selectable.selectedObjects.Clear();
	}
}
