using UnityEngine;
using System.Collections;

public struct SpawnUnit {
	public GameObject owner;
	public GameObject spawnedUnit;
	public float time;

	public SpawnUnit(GameObject owner, GameObject unit, float time) {
		this.owner = owner;
		this.spawnedUnit = unit;
		this.time = time;
	}

	public void Clear() {
		this.owner = null;
		this.spawnedUnit = null;
		this.time = 0f;
	}
};

public class NewDivision : MonoBehaviour {
	private Selectable ownerSelectable;
	private Selectable spawnedSelectable;
	private Color divisionColor = Color.cyan;
	private Color initialColor = Color.white;
	private NetworkView playerNetworkView;
	private float cooldownTimer = 15f;
	private bool isReady;
	private SpawnUnit spawnedUnit;
	private Vector3 spawnedLocation = Vector3.zero;
	private Vector3 rotatedVector = Vector3.zero;
	private float elapsedTime;


	public void Start() {
		this.ownerSelectable = this.GetComponent<Selectable>();
		this.isReady = true;
		this.playerNetworkView = this.GetComponent<NetworkView>();
		this.elapsedTime = 0f;
	}

	public void Update() {
		if (!this.isReady) {
			if (this.elapsedTime < 1f) {
				this.StartCoroutine(CR_MoveToPosition(this.spawnedLocation + rotatedVector));
				this.StartCoroutine(CR_CooldownTime());
			}
		}
	}

	public void OnGUI() {
		if (this.ownerSelectable != null && this.playerNetworkView.isMine) {
			if (Input.GetKeyDown(KeyCode.S) && this.ownerSelectable.isSelected && this.isReady) {
				this.spawnedLocation = this.gameObject.transform.position;
				GameObject unit = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.spawnedLocation, Quaternion.identity, 0);
				unit.name = unit.name + " (Spawned)";
				this.spawnedSelectable = unit.GetComponentInChildren<Selectable>();
				this.spawnedSelectable.DisableSelection();
				this.ownerSelectable.DisableSelection();
				this.spawnedUnit = new SpawnUnit(this.gameObject, unit, this.cooldownTimer);

				this.isReady = false;
				this.elapsedTime = 0f;
				rotatedVector = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f) * new Vector3(1f, 0f, 0f);
			}
		}
	}

	private IEnumerator CR_MoveToPosition(Vector3 target) {
		while (this.elapsedTime < 1f) {
			this.transform.position = Vector3.Lerp(this.spawnedLocation, target, this.elapsedTime);
			Vector3 negativeTarget = -target;
			negativeTarget.y = target.y;
			this.spawnedUnit.spawnedUnit.transform.position = Vector3.Lerp(this.spawnedLocation, negativeTarget, this.elapsedTime);
			this.elapsedTime += Time.deltaTime / this.cooldownTimer;
			yield return null;
		}
	}

	public IEnumerator CR_CooldownTime() {
		Renderer rendererA = this.spawnedUnit.owner.GetComponentInChildren<Renderer>();
		Renderer rendererB = this.spawnedUnit.spawnedUnit.GetComponentInChildren<Renderer>();
		while (this.elapsedTime < 1f) {
			bool halfTime = this.elapsedTime < 0.5f;
			if (halfTime) {
				rendererA.material.color = Color.Lerp(this.initialColor, this.divisionColor, this.elapsedTime);
				rendererB.material.color = Color.Lerp(this.initialColor, this.divisionColor, this.elapsedTime);
			}
			else {
				rendererA.material.color = Color.Lerp(this.divisionColor, this.initialColor, this.elapsedTime);
				rendererB.material.color = Color.Lerp(this.divisionColor, this.initialColor, this.elapsedTime);
			}
			yield return null;
		}
		if (!this.ownerSelectable.IsSelectionEnabled()) {
			this.ownerSelectable.EnableSelection();
			this.spawnedSelectable.EnableSelection();
		}
		this.isReady = true;
		rendererA.material.color = this.initialColor;
	}
}
