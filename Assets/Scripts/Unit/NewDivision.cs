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
		this.elapsedTime = this.cooldownTimer;
	}

	public void Update() {
		if (this.ownerSelectable != null && this.playerNetworkView.isMine) {
			if (Input.GetKeyDown(KeyCode.S) && this.ownerSelectable.isSelected && this.isReady) {
				this.spawnedLocation = this.gameObject.transform.position;
				GameObject unit = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.spawnedLocation, Quaternion.identity, 0);
				unit.name = unit.name + " (Spawned)";
				UnitManager.instance.PlayerUnits.Add(unit);
				this.spawnedSelectable = unit.GetComponentInChildren<Selectable>();
				this.spawnedSelectable.DisableSelection();
				this.ownerSelectable.DisableSelection();
				this.spawnedUnit = new SpawnUnit(this.gameObject, unit, this.cooldownTimer);

				this.isReady = false;
				this.elapsedTime = 0f;
				rotatedVector = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f) * new Vector3(1f, 0f, 0f);
			}
		}

		if (!this.isReady) {
			if (this.elapsedTime < this.cooldownTimer) {
				this.StartCoroutine(CR_MoveToPosition(this.spawnedLocation + rotatedVector));
				this.StartCoroutine(CR_CooldownTime());
			}
		}
	}

	private IEnumerator CR_MoveToPosition(Vector3 target) {
		while (elapsedTime < this.cooldownTimer) {
			this.transform.position = Vector3.Lerp(this.spawnedLocation, target, (elapsedTime) / this.cooldownTimer);
			Vector3 negativeTarget = -target;
			negativeTarget.y = target.y;
			this.spawnedUnit.spawnedUnit.transform.position = Vector3.Lerp(this.spawnedLocation, negativeTarget, (elapsedTime) / this.cooldownTimer);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	public IEnumerator CR_CooldownTime() {
		Renderer rendererA = this.spawnedUnit.owner.GetComponentInChildren<Renderer>();
		Renderer rendererB = this.spawnedUnit.spawnedUnit.GetComponentInChildren<Renderer>();
		while (this.elapsedTime < this.cooldownTimer) {
			bool halfTime = (this.elapsedTime < this.cooldownTimer / 2f);
			float lerpTime = (this.elapsedTime) / this.cooldownTimer;
			if (halfTime) {
				rendererA.material.color = Color.Lerp(this.initialColor, this.divisionColor, lerpTime);
				rendererB.material.color = Color.Lerp(this.initialColor, this.divisionColor, lerpTime);
			}
			else {
				rendererA.material.color = Color.Lerp(this.divisionColor, this.initialColor, lerpTime);
				rendererB.material.color = Color.Lerp(this.divisionColor, this.initialColor, lerpTime);
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
