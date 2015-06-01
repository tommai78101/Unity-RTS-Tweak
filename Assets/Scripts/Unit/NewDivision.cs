using UnityEngine;
using System.Collections;

public class NewDivision : MonoBehaviour {
	private Selectable selectable;
	private Color divisionColor = Color.cyan;
	private Color initialColor = Color.white;
	private NetworkView playerNetworkView;
	private float cooldownTimer = 15f;
	private bool isReady;

	public void Start() {
		this.selectable = this.GetComponent<Selectable>();
		this.isReady = true;
		this.playerNetworkView = this.GetComponent<NetworkView>();
	}

	public void Update() {
		if (this.selectable != null && this.playerNetworkView.isMine) {
			if (Input.GetKeyDown(KeyCode.S) && this.selectable.isSelected && this.isReady) {
				GameObject spawnedUnit = (GameObject) Network.Instantiate(Resources.Load("Prefabs/Player"), this.gameObject.transform.position, Quaternion.identity, 0);
				spawnedUnit.name = spawnedUnit.name + " (Spawned)";
				this.isReady = false;
				this.selectable.DisableSelection();
			}
		}

		if (!this.isReady) {
			Vector3 rotatedVector = Quaternion.Euler(0f, 0f, Random.RandomRange(-180f, 180f)) * new Vector3(1f, 0f, 0f);
			this.StartCoroutine(CR_MoveToPosition(this.transform.position + rotatedVector));
			this.StartCoroutine(CR_CooldownTime());
		}
	}

	private IEnumerator CR_MoveToPosition(Vector3 target) {
		float elapsedTime = 0f;
		Vector3 startingPosition = this.transform.position;
		while (elapsedTime < this.cooldownTimer) {
			this.transform.position = Vector3.Lerp(startingPosition, target, (elapsedTime) / this.cooldownTimer);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	public IEnumerator CR_CooldownTime() {
		Renderer renderer = this.GetComponentInChildren<Renderer>();
		float EndTime = Time.time + cooldownTimer;
		bool halfTime = (Time.time < Time.time + (cooldownTimer / 2f) ? true : false);
		while (Time.time < EndTime) {
			if (halfTime) {
				renderer.material.color = Color.Lerp(this.initialColor, this.divisionColor, (EndTime - Time.time) / cooldownTimer);
			}
			else {
				renderer.material.color = Color.Lerp(this.divisionColor, this.initialColor, (EndTime - Time.time) / cooldownTimer);
			}
			yield return new WaitForEndOfFrame();
		}
		if (!this.selectable.IsSelectionEnabled()) {
			this.selectable.EnableSelection();
		}
		this.isReady = true;
		renderer.material.color = this.initialColor;
	}
}
