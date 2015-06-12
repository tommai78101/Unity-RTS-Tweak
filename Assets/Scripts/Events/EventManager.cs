using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {
	//Singleton
	private static EventManager instance;
	private EventManager() {}
	public static EventManager Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindObjectOfType(typeof(EventManager)) as EventManager;
				return instance;
			}
			return instance;
		}
	}

	//Events
	//Events must be void and have 0 or 1 parameter.
	public delegate void OnLeftClickEvent(GameObject go);
	public event OnLeftClickEvent OnLeftClickUp;
	public event OnLeftClickEvent OnLeftClickHold;
	public event OnLeftClickEvent OnLeftClickDown;

	public void Update() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100f)) {
			if (Input.GetMouseButtonUp(0)){
				if (OnLeftClickUp != null) {
					OnLeftClickUp(hit.transform.gameObject);
				}
			}
			else if (Input.GetMouseButtonDown(0)){
				if (OnLeftClickDown != null) {
					OnLeftClickDown(hit.transform.gameObject);
				}
			}
			else if (Input.GetMouseButton(0)) {
				if (OnLeftClickHold != null) {
					OnLeftClickHold(hit.transform.gameObject);
				}
			}
		}
	}

	public void OnDestroy() {
		EventManager.instance = null;
	}
}
