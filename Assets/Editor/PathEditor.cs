using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {
    private PathCreator _Creator;
    private Path _Path;

    public void OnSceneGUI() {
        Input();
        Draw();
    }

    private void Input() {
        Event guiEvent = Event.current;
        Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && _Creator.Edit && guiEvent.button == 0 && guiEvent.shift) {
            Undo.RecordObject(_Creator, "Add segment");
            _Path.AddSegment(mousePosition);
        }
    }

    private void Draw() {
        Handles.color = new Color(0, .5f, 0);

        // Draw curve
        for (int i = 0; i < _Path.NumSegments; i++) {
            Vector2[] points = _Path.GetPointsInSegment(i);

            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);

            if (_Creator.Edit) {
                Handles.DrawLine(points[0], points[1]);
                Handles.DrawLine(points[2], points[3]);
            }
        }

        // Draw handles
        if (_Creator.Edit) {
            for (int i = 0; i < _Path.NumPoints; i++) {
                Vector2 newPosition = Handles.FreeMoveHandle(_Path[i], Quaternion.identity, .025f, Vector2.zero, Handles.RectangleHandleCap);

                if (_Path[i] != newPosition) {
                    Undo.RecordObject(_Creator, "Move point");
                    _Path.MovePoint(i, newPosition);
                }
            }
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
