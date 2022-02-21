using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {
    private PathCreator _Creator;
    private Path _Path;
    private int _SelectedPoint = -1;

    public void OnSceneGUI() {
        Input();
        Draw();
    }

    private void Input() {
        Event guiEvent = Event.current;
        Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (_Creator.Edit && guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
            // Get selected point.
            int selectedPoint = _SelectedPoint;
            _SelectedPoint = -1;
            for (int i = 0; i < _Path.NumPoints; i++) {
                if (Vector2.Distance(mousePosition, _Path[i]) < _Creator.PointRadius * 2) {
                    if (i % 3 == 0) {
                        _SelectedPoint = i;
                        break;
                    } else if (i == selectedPoint + 1 || i == selectedPoint - 1) {
                        _SelectedPoint = selectedPoint;
                        break;
                    }
                }
            }

            // Add new segment.
            if (guiEvent.shift) {
                Undo.RecordObject(_Creator, "Add segment");
                _Path.AddSegment(mousePosition);

                _SelectedPoint = _Path.NumPoints - 1;
            }
        }
    }

    private void Draw() {
        Handles.color = new Color(0, .5f, 0);

        // Draw curve
        for (int i = 0; i < _Path.NumSegments; i++) {
            Vector2[] points = _Path.GetPointsInSegment(i);

            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
        }

        if (_Creator.Edit) {
            // Draw points
            Handles.color = new Color(0, .5f, 0);
            for (int i = 0; i < _Path.NumPoints; i++) {
                if (i % 3 == 0) {
                    DrawPoint(i);
                }
            }

            // Draw selected point and handles.
            Handles.color = Color.white;
            DrawSelected();
        }
    }

    private void DrawPoint(int i) {
        Vector2 newPosition = Handles.FreeMoveHandle(_Path[i], Quaternion.identity, _Creator.PointRadius, Vector2.zero, Handles.CubeHandleCap);

        if (_Path[i] != newPosition) {
            Undo.RecordObject(_Creator, "Move point");
            _Path.MovePoint(i, newPosition);
        }
    }

    private void DrawSelected() {
        if (_SelectedPoint < 0) {
            return;
        }

        DrawPoint(_SelectedPoint);

        if (_SelectedPoint + 1 < _Path.NumPoints) {
            Handles.DrawLine(_Path[_SelectedPoint], _Path[_SelectedPoint + 1]);
            DrawPoint(_SelectedPoint + 1);
        }

        if (_SelectedPoint - 1 >= 0) {
            Handles.DrawLine(_Path[_SelectedPoint], _Path[_SelectedPoint - 1]);
            DrawPoint(_SelectedPoint - 1);
        }
    }

    private void OnEnable() {
        _Creator = target as PathCreator;
        _Creator.Edit = false;

        if (_Creator.Path == null) {
            _Creator.CreatePath();
        }

        _Path = _Creator.Path;
    }
}
