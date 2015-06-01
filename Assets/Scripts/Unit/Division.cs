using UnityEngine;
using System.Collections;

public class Division : MonoBehaviour {
	public static long id;
	public GameObject target;
	
	private bool InstantiateCompleteFlag;
	private bool CannotDivideFlag;
	private bool isEnabled = false;

	// Use this for initialization
	void Start () {
		InstantiateCompleteFlag = true;
		StartCoroutine(Wait (this.gameObject));

		this.isEnabled = false;
		NetworkView playerNetworkView = this.GetComponent<NetworkView>();
		if (playerNetworkView != null && playerNetworkView.isMine) {
			this.isEnabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.isEnabled) {
			return;
		}
		if (Input.GetMouseButton(1) && !InstantiateCompleteFlag){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100) && Vector3.Distance(hit.point, hit.collider.gameObject.transform.position) <= 0.65f) {
				NavMeshAgent agent = null;

				if (!InstantiateCompleteFlag){
					if (!this.CannotDivideFlag) {

						//newObject = (GameObject) Object.Instantiate(target);
						//newObject.name = "Player " + id++;
						//agent = newObject.GetComponent<NavMeshAgent>();

						NetworkView networkView = this.GetComponent<NetworkView>();
						if (networkView != null) {
							Vector3 pos = hit.collider.gameObject.transform.position;
							networkView.RPC("RPC_SpawnUnit", RPCMode.AllBuffered, new object[]{ pos });
						}
					}
					InstantiateCompleteFlag = true;
				}
				 
				if (agent){
					Vector3 newPosition = target.transform.position;
					newPosition.x += ((Random.value*2f - 1f) * (Random.value + 1f));
					newPosition.z += ((Random.value*2f - 1f) * (Random.value + 1f));
					agent.SetDestination(newPosition);
				}
				
				
				//if (newObject && InstantiateCompleteFlag) {
				//	Debug.Log("Starting co-routines.");
				//	StartCoroutine(Wait (newObject));
				//	StartCoroutine(Wait (this.gameObject));
				//}
			}
		}

	}
	
	IEnumerator Wait(GameObject obj){
		yield return new WaitForSeconds(5f);
		Division div = obj.GetComponent<Division>();
		div.setInstantiateFlag(false);
	}
	
	public void setInstantiateFlag(bool value){
		this.InstantiateCompleteFlag = value;
	}

	public void SetCannotDivideFlag() {
		this.CannotDivideFlag = true;
	}

	[RPC]
	public void RPC_SpawnUnit(Vector3 location) {
		if (this.target != null) {
			GameObject newObject = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), location, Quaternion.identity, UnitManager.PG_Enemy);
			newObject.name = "Enemy " + id++;

			UnitManager.instance.AllUnits.Add(newObject);

			Division div = newObject.GetComponent<Division>();
			if (div != null) {
				div.setInstantiateFlag(false);
			}

			Selectable selectable = newObject.GetComponent<Selectable>();
			if (selectable != null) {
				selectable.SelectableID = (int) id++;		
				selectable.UUID = System.Guid.NewGuid();
			}
		}
	}
}
