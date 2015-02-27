using UnityEngine;
using System.Collections;

public class Division : MonoBehaviour {

	public GameObject target;

	bool InstantiateCompleteFlag;

	// Use this for initialization
	void Start () {
		InstantiateCompleteFlag = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100) && Vector3.Distance(hit.point, target.transform.position) <= 0.65f){
				NavMeshAgent agent = null;

				if (!InstantiateCompleteFlag){
					GameObject newObject = (GameObject) Instantiate(target);
					agent = newObject.GetComponent<NavMeshAgent>();
					InstantiateCompleteFlag = true;
				}
				 
				if (agent){
					Vector3 newPosition = target.transform.position;
					newPosition.x += ((Random.value*2f - 1f) * (Random.value + 1f));
					newPosition.z += ((Random.value*2f - 1f) * (Random.value + 1f));
					agent.SetDestination(newPosition);
				}
			}
		}
	}
}
