using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Common;

public class PrefabInitialization : NetworkBehaviour {
	public static PrefabInitialization Instance;

	public List<GameObject> gameObjectList;
	public CommonUnitManager unitManager;
	public CommonInputManager inputManager;
	public CommonAttackManager attackManager;
	public CommonMergeManager mergeManager;
	public CommonSplitManager splitManager;

	public void Awake() {
		PrefabInitialization.Instance = this;
	}

	private void SpawnObject(int i) {
		GameObject obj = GameObject.Instantiate(this.gameObjectList[i], Vector3.zero, Quaternion.identity) as GameObject;
		obj.name = obj.name.Substring(0, obj.name.Length - "(Clone)".Length);
		if (obj.tag.Equals("Input_Manager")) {
			this.inputManager = obj.GetComponent<CommonInputManager>();
			this.inputManager.attackManager = this.attackManager;
			this.inputManager.splitManager = this.splitManager;
			this.inputManager.mergeManager = this.mergeManager;
			this.inputManager.unitManager = this.unitManager;
		}
		else if (obj.tag.Equals("Merge_Manager")) {
			this.mergeManager = obj.GetComponent<CommonMergeManager>();
			this.mergeManager.unitManager = this.unitManager;
		}
		else if (obj.tag.Equals("Split_Manager")) {
			this.splitManager = obj.GetComponent<CommonSplitManager>();
		}
		else if (obj.tag.Equals("Attack_Manager")) {
			this.attackManager = obj.GetComponent<CommonAttackManager>();
		}
		else if (obj.tag.Equals("Unit_Manager")) {
			this.unitManager = obj.GetComponent<CommonUnitManager>();
		}
		else if (obj.tag.Equals("RPCLess_Unit")) {
			CommonUnit commonUnit = obj.GetComponent<CommonUnit>();
			commonUnit.transform.position = this.transform.position;
			commonUnit.unitManager = this.unitManager;
			if (!this.unitManager.getAllObjects().Contains(obj)) {
				this.unitManager.getAllObjects().Add(obj);
			}
		}
		NetworkServer.Spawn(obj);
	}

	public override void OnStartLocalPlayer() {
		for (int i = 0; i < this.gameObjectList.Count; i++) {
			SpawnObject(i);
		}
		for (int i = 0; i < this.gameObjectList.Count; i++) {
			this.gameObjectList[i].SetActive(true);
		}
	}
}
