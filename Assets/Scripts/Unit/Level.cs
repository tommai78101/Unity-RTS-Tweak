using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {
	public const int GREATER_THAN = 1;
	public const int EQUALS = 0;
	public const int LESS_THAN = -1;
	public const int NULL = -99;

	public int levelNumber;

	private void Start() {
		this.levelNumber = 1;
	}

	public int Compare(GameObject other) {
		Level level = other.GetComponent<Level>();
		if (level != null) {
			if (this.levelNumber < level.levelNumber) {
				return LESS_THAN;
			}
			else if (this.levelNumber > level.levelNumber) {
				return GREATER_THAN;
			}
			return EQUALS;
		}
		else {
			return NULL;
		}
	}

	public int Compare(Level level) {
		if (level == null){
			return NULL;
		}
		if (this.levelNumber < level.levelNumber) {
			return LESS_THAN;
		}
		else if (this.levelNumber > level.levelNumber) {
			return GREATER_THAN;
		}
		return EQUALS;
	}

	public void IncrementLevel() {
		this.levelNumber++;
	}

	public void DecrementLevel() {
		this.levelNumber--;
	}
}
