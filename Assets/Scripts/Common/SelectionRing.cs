using UnityEngine;
using System.Collections;

namespace Common {
	public class SelectionRing : MonoBehaviour {
		public bool isSelected;
		public MeshRenderer meshRenderer;
		public Color color;

		protected void Start() {
			this.isSelected = false;
			this.meshRenderer = this.GetComponent<MeshRenderer>();
		}

		protected void Update() {
			if (this.meshRenderer != null) {
				if (this.isSelected) {
					this.meshRenderer.enabled = true;
					this.meshRenderer.material.color = this.color;
				}
				else {
					this.meshRenderer.enabled = false;
				}
			}
		}

		public void SetColor(Color color) {
			this.color = color;
		}
	}
}
