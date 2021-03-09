using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
public class CanvasPositionHolder : MonoBehaviour {
    public Vector2 position;
}
