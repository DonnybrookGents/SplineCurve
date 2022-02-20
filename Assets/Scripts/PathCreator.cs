using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {
    [HideInInspector] public Path Path;
    public bool Edit;
    public AnimationCurve Test;

    public void CreatePath() {
        Path = new Path(transform.position);
    }
}
