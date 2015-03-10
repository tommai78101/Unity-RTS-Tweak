﻿using UnityEngine;
using System.Collections;


public class Division : MonoBehaviour {

	static long id;

	public GameObject target;
	
	bool InstantiateCompleteFlag;

	// Use this for initialization
	void Start () {
		InstantiateCompleteFlag = true;
		StartCoroutine(Wait (this.gameObject));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1) && !InstantiateCompleteFlag){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100) && Vector3.Distance(hit.point, target.transform.position) <= 0.65f){
				NavMeshAgent agent = null;
				GameObject newObject = null;

				if (!InstantiateCompleteFlag){
					newObject = (GameObject) Instantiate(target);
					newObject.name = "Player " + id++;
					agent = newObject.GetComponent<NavMeshAgent>();
					
					InstantiateCompleteFlag = true;
				}
				 
				if (agent){
					Vector3 newPosition = target.transform.position;
					newPosition.x += ((Random.value*2f - 1f) * (Random.value + 1f));
					newPosition.z += ((Random.value*2f - 1f) * (Random.value + 1f));
					agent.SetDestination(newPosition);
				}
				
				
				if (newObject && InstantiateCompleteFlag) {
					Debug.Log("Starting co-routines.");
					StartCoroutine(Wait (newObject));
					StartCoroutine(Wait (this.gameObject));
				}
			}
		}

	}
	
	IEnumerator Wait(GameObject obj){
		Debug.Log("Waiting 5 secs - " + obj.name);
		yield return new WaitForSeconds(5f);
		Division div = obj.GetComponent<Division>();
		div.setInstantiateFlag(false);
		Debug.Log("Set flag to false. " + obj.name);
	}
	
	public void setInstantiateFlag(bool value){
		this.InstantiateCompleteFlag = value;
	}
}