using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path {
    [SerializeField, HideInInspector] List<Vector2> Points;

    public Vector2 this[int i] {
        get {
            return Points[i];
        }
    }

    public int NumPoints {
        get {
            return Points.Count;
        }
    }

    public int NumSegments {
        get {
            return ((Points.Count - 4) / 3) + 1;
        }
    }

    public Path(Vector2 center) {
        Points = new List<Vector2> {
            center + Vector2.left,
            center + (Vector2.left + Vector2.up) * .5f,
            center + (Vector2.right + Vector2.down) * .5f,
            center + Vector2.right
        };
    }

    public void AddSegment(Vector2 anchorPosition) {
        Points.Add(Points[Points.Count - 1] * 2 - Points[Points.Count - 2]);
        Points.Add((Points[Points.Count - 1] + anchorPosition) * .5f);
        Points.Add(anchorPosition);
    }

    public Vector2[] GetPointsInSegment(int i) {
        return new Vector2[]{
            Points[i * 3],
            Points[(i * 3) + 1],
            Points[(i * 3) + 2],
            Points[(i * 3) + 3]
        };
    }

    public void MovePoint(int i, Vector2 position) {
        Vector2 deltaMove = position - Points[i];
        Points[i] = position;

        // Move handles with anchor.
        if (i % 3 == 0) {
            if (i + 1 < Points.Count) {
                Points[i + 1] += deltaMove;
            }

            if (i - 1 >= 0) {
                Points[i - 1] += deltaMove;
            }
        } else {
            bool nextPointAnchor = (i + 1) % 3 == 0;

            int anchorIndex = nextPointAnchor ? i + 1 : i - 1;
            int handleIndex = nextPointAnchor ? i + 2 : i - 2;

            if (handleIndex >= 0 && handleIndex < Points.Count) {
                float distance = (Points[anchorIndex] - Points[handleIndex]).magnitude;
                Vector2 direction = (Points[anchorIndex] - position).normalized;

                Points[handleIndex] = Points[anchorIndex] + direction * distance;
            }
        }
    }
}
