using UnityEngine;
using System.Collections;

namespace Extension {
	public static class ExtensionClass {
		public static bool ReachedDestination(this NavMeshAgent agent) {
			if (!agent.pathPending) {
				if (agent.remainingDistance <= agent.stoppingDistance) {
					if (!agent.hasPath || agent.velocity.sqrMagnitude <= float.Epsilon) {
						return true;
					}
				}
			}
			return false;
		}

		public static Vector2 NormalizePosition(this Vector2 vector) {
			return new Vector2(vector.x / Screen.width, vector.y / Screen.height);
		}

		public static Vector2 UnnormalizePosition(this Vector2 vector) {
			return new Vector2(vector.x * Screen.width, vector.y * Screen.height);
		}

		public static bool Similar(this float value, float other) {
			float delta = Mathf.Abs(value - other);
			if (delta <= float.Epsilon) {
				return true;
			}
			return false;
		}

		public static bool IsEmpty(this string value) {
			if (value.Length <= 0 || value == null) {
				return true;
			}
			return false;
		}
	}
}
