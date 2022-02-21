using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path {
    [HideInInspector] public bool IsClosed;
    [SerializeField, HideInInspector] private List<Vector2> Points;

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
            return Points.Count / 3;
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
            Points[LoopIndex((i * 3) + 3)]
        };
    }

    public void MovePoint(int i, Vector2 position) {
        Vector2 deltaMove = position - Points[i];
        Points[i] = position;

        // Move handles with anchor.
        if (i % 3 == 0) {
            if (i + 1 < Points.Count || IsClosed) {
                Points[LoopIndex(i + 1)] += deltaMove;
            }

            if (i - 1 >= 0 || IsClosed) {
                Points[LoopIndex(i - 1)] += deltaMove;
            }
        } else {
            bool nextPointAnchor = (i + 1) % 3 == 0;

            int anchorIndex = nextPointAnchor ? i + 1 : i - 1;
            int handleIndex = nextPointAnchor ? i + 2 : i - 2;

            if ((handleIndex >= 0 && handleIndex < Points.Count) || IsClosed) {
                float distance = (Points[LoopIndex(anchorIndex)] - Points[LoopIndex(handleIndex)]).magnitude;
                Vector2 direction = (Points[LoopIndex(anchorIndex)] - position).normalized;

                Points[LoopIndex(handleIndex)] = Points[LoopIndex(anchorIndex)] + direction * distance;
            }
        }
    }

    public void ToggleClosed() {
        IsClosed = !IsClosed;

        if (IsClosed) {
            Points.Add(Points[Points.Count - 1] * 2 - Points[Points.Count - 2]);
            Points.Add(Points[0] * 2 - Points[1]);
        } else {
            Points.RemoveRange(Points.Count - 2, 2);
        }
    }

    public int LoopIndex(int i) {
        return (i + Points.Count) % Points.Count;
    }
}
