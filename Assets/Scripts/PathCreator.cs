using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {
    [HideInInspector] public float PointRadius = 0.025f;
    [HideInInspector] public Path Path;

    public void CreatePath() {
        Path = new Path(transform.position);
    }
}
