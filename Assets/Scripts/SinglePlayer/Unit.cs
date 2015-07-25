using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum OrderType {
	ATTACK, SPLIT, MOVE, MERGE, NONE
}

public struct CommandOrder {
	Vector3 orderPosition;
	Vector3 targetPosition;
	OrderType orderType;
	int priority;
}

public class HealthSystem {
	public Unit owner;
	public int maxHealth;
	public int currenltHealth;

	public HealthSystem(Unit unit, int max) {
		this.owner = unit;
		this.maxHealth = max;
		this.currenltHealth = this.maxHealth;
	}
}

public abstract class Unit : MonoBehaviour{
	public Queue<CommandOrder> commandQueue = new Queue<CommandOrder>();
	public HealthSystem healthSystem;
	public GameObject inputHandler;

	protected void Start() {
		this.healthSystem = new HealthSystem(this, 5);

	}

	protected void Update() {
		
	}
}