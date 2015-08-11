using UnityEngine;
using System.Collections;

namespace Tutorial {
	public class TutorialRing : MonoBehaviour {
		public bool isSelected;
		public MeshRenderer meshRenderer;
		public Color color;

		// Use this for initialization
		void Start() {
			this.isSelected = false;
			this.meshRenderer = this.GetComponent<MeshRenderer>();
		}

		// Update is called once per frame
		void Update() {
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
