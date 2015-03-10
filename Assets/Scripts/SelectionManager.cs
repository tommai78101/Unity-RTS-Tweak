using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {
	private sealed class Dummy {
		public static Dummy Instance;

		private Object lockObject = new Object();
		private SelectionManager selectionManager;
		private Vector3 initialClick;

		public Dummy(SelectionManager manager) {
			if (Instance == null) {
				lock (lockObject) {
					if (Instance == null) {
						this.selectionManager = manager;
						initialClick = -Vector3.one;
						Dummy.Instance = this;
					}
				}
			}
			return;
		}

		public void update() {
			if (Input.GetMouseButtonDown(0)) {
				this.initialClick = Input.mousePosition;
			}
			else if (Input.GetMouseButtonUp(0)) {
				this.initialClick = -Vector3.one;
			}

			if (Input.GetMouseButton(0)) {
				SelectionManager.selectionArea.Set(initialClick.x, Screen.height - initialClick.y, Input.mousePosition.x - initialClick.x, (Screen.height - Input.mousePosition.y) - (Screen.height - initialClick.y));
				if (SelectionManager.selectionArea.width < 0) {
					SelectionManager.selectionArea.x += SelectionManager.selectionArea.width;
					SelectionManager.selectionArea.width *= -1f;
				}
				if (SelectionManager.selectionArea.height < 0) {
					SelectionManager.selectionArea.y += SelectionManager.selectionArea.height;
					SelectionManager.selectionArea.height *= -1f;
				}
			}
		}

		private void onGUI() {
			if (initialClick != -Vector3.one) {
				//GUI.color = new Color(1, 1, 1, 0.5f);
				//GUI.DrawTexture(SelectionManager.selectionArea, this.selectionManager.selectionTexture);
			}
		}

		public Vector3 getInitialVertex() {
			float x = Input.mousePosition.x < this.initialClick.x ? Input.mousePosition.x : this.initialClick.x;
			float y = Input.mousePosition.y < this.initialClick.y ? Input.mousePosition.y : this.initialClick.y;
			x /= Screen.width;
			y /= Screen.height;
			return new Vector3(x, y, 0f);
		}

		public Vector3 getEndingVertex() {
			float x = Input.mousePosition.x < this.initialClick.x ? this.initialClick.x : Input.mousePosition.x;
			float y = Input.mousePosition.y < this.initialClick.y ? this.initialClick.y : Input.mousePosition.y;
			x /= Screen.width;
			y /= Screen.height;
			return new Vector3(x, y, 0f);
		}
	}


	public static Rect selectionArea = new Rect();

	public Texture2D selectionTexture;
	public Material borderMaterial;

	private Vector3 startingVertex;
	private Vector3 endingVertex;
	private Color borderColor;

	private Dummy dummyObject;

	// Use this for initialization
	void Start() {
		this.startingVertex = this.endingVertex = -Vector3.one;
		this.borderColor = new Color(1f, 128f / 255f, 1f, 1f);
		this.dummyObject = new Dummy(this);
	}

	// Update is called once per frame
	void Update() {
		this.dummyObject.update();
		this.startingVertex = this.dummyObject.getInitialVertex();
		this.endingVertex = this.dummyObject.getEndingVertex();
	}

	void OnPostRender() {
		if (!borderMaterial) {
			Debug.LogError("Might need a material here.");
			return;
		}

		if (Input.GetMouseButton(0)) {
			GL.PushMatrix();
			borderMaterial.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(GL.LINES);
			//Vertices uses normalized coordinates, and not absolute values.
			GL.Color(this.borderColor);
				GL.Vertex(this.startingVertex);
				GL.Vertex(new Vector3(this.endingVertex.x, this.startingVertex.y, 0f));
				//--------
				GL.Vertex(new Vector3(this.endingVertex.x, this.startingVertex.y, 0f));
				GL.Vertex(this.endingVertex);
				//--------
				GL.Vertex(this.endingVertex);
				GL.Vertex(new Vector3(this.startingVertex.x, this.endingVertex.y, 0f));
				//--------
				GL.Vertex(new Vector3(this.startingVertex.x, this.endingVertex.y, 0f));
				GL.Vertex(this.startingVertex);
			GL.End();
			GL.PopMatrix();
		}
	}
}
