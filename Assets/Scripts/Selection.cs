using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selection : MonoBehaviour {
	public static Rect selectionArea = new Rect();
	public Material borderMaterial;

	private Vector3 initialClick;
	private Vector3 startingVertex;
	private Vector3 endingVertex;
	private Color borderColor;

	// Use this for initialization
	void Start() {
		this.initialClick = this.startingVertex = this.endingVertex = -Vector3.one;
		this.borderColor = new Color(1f, 128f / 255f, 1f, 1f); //Dark green
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			this.initialClick = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0)) {
			this.initialClick = -Vector3.one;
		}

		if (Input.GetMouseButton(0)) {
			Selection.selectionArea.Set(this.initialClick.x, Screen.height - this.initialClick.y, Input.mousePosition.x - this.initialClick.x, (Screen.height - Input.mousePosition.y) - (Screen.height - this.initialClick.y));
			if (Selection.selectionArea.width < 0) {
				Selection.selectionArea.x += Selection.selectionArea.width;
				Selection.selectionArea.width *= -1f;
			}
			if (Selection.selectionArea.height < 0) {
				Selection.selectionArea.y += Selection.selectionArea.height;
				Selection.selectionArea.height *= -1f;
			}
		}
		this.startingVertex = getStartingVertex();
		this.endingVertex = getEndingVertex();
	}

	void OnPostRender() {
		if (!borderMaterial) {
			Debug.LogError("Might need a material here.");
			return;
		}

		if (this.startingVertex == -Vector3.one || this.endingVertex == -Vector3.one) {
			return;
		}

		if (Input.GetMouseButton(0)) {
			GL.PushMatrix();
			if (borderMaterial.SetPass(0)) {
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
			}
			GL.PopMatrix();
		}
	}

	private Vector3 getStartingVertex() {
		if (this.initialClick == -Vector3.one) {
			return -Vector3.one;
		}
		float y = Input.mousePosition.y < this.initialClick.y ? Input.mousePosition.y : this.initialClick.y;
		float x = Input.mousePosition.x < this.initialClick.x ? Input.mousePosition.x : this.initialClick.x;
		x /= Screen.width;
		y /= Screen.height;
		return new Vector3(x, y, 0f);
	}

	private Vector3 getEndingVertex() {
		if (this.initialClick == -Vector3.one) {
			return -Vector3.one;
		}
		float x = Input.mousePosition.x < this.initialClick.x ? this.initialClick.x : Input.mousePosition.x;
		float y = Input.mousePosition.y < this.initialClick.y ? this.initialClick.y : Input.mousePosition.y;
		x /= Screen.width;
		y /= Screen.height;
		return new Vector3(x, y, 0f);
	}
}
