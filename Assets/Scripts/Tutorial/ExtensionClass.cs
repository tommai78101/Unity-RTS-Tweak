using UnityEngine;
using System.Collections;

namespace Tutorial {
	public static class ExtensionClass {
		public static bool reachedDestination(this NavMeshAgent agent) {
			if (!agent.pathPending) {
				if (agent.remainingDistance <= agent.stoppingDistance) {
					if (!agent.hasPath || agent.velocity.sqrMagnitude <= float.Epsilon) {
						return true;
					}
				}
			}
			return false;
		}

		public static Vector2 normalizePosition(this Vector2 vector) {
			return new Vector2(vector.x / Screen.width, vector.y / Screen.height);
		}

		public static Vector2 unnormalizePosition(this Vector2 vector) {
			return new Vector2(vector.x * Screen.width, vector.y * Screen.height);
		}
	}
}
