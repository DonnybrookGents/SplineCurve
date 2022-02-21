using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine.AI;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {
    private PathCreator _Creator;
    private Path _Path;
    private bool Edit = false;
    private bool IsClosed;
    private int _SelectedPoint = -1;
    private GUIContent _EditModeButton;

    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Edit Spline", GUILayout.Width(130));
        if (GUILayout.Button(_EditModeButton, GUILayout.Width(33), GUILayout.Height(23))) {
            Edit = !Edit;
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();

        IsClosed = EditorGUILayout.Toggle("Closed Loop", IsClosed);
        if (_Path.IsClosed != IsClosed) {
            _Path.ToggleClosed();
            SceneView.RepaintAll();

            IsClosed = _Path.IsClosed;
        }
    }

    public void OnSceneGUI() {
        Input();
        Draw();
    }

    public void Reset() {
        Debug.Log("reset");
    }

    private void Input() {
        Event guiEvent = Event.current;
        Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (Edit && guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
            // Get selected point.
            int selectedPoint = _SelectedPoint;
            _SelectedPoint = -1;
            for (int i = 0; i < _Path.NumPoints; i++) {
                if (Vector2.Distance(mousePosition, _Path[i]) < _Creator.PointRadius * 2) {
                    if (i % 3 == 0) {
                        _SelectedPoint = i;
                        break;
                    } else if (i == _Path.LoopIndex(selectedPoint + 1) || i == _Path.LoopIndex(selectedPoint - 1)) {
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

            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, Edit ? 2 : 1);
        }

        if (Edit) {
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

        if (_SelectedPoint + 1 < _Path.NumPoints || _Path.IsClosed) {
            Handles.DrawLine(_Path[_SelectedPoint], _Path[_Path.LoopIndex(_SelectedPoint + 1)]);
            DrawPoint(_Path.LoopIndex(_SelectedPoint + 1));
        }

        if (_SelectedPoint - 1 >= 0 || _Path.IsClosed) {
            Handles.DrawLine(_Path[_SelectedPoint], _Path[_Path.LoopIndex(_SelectedPoint - 1)]);
            DrawPoint(_Path.LoopIndex(_SelectedPoint - 1));
        }
    }

    private void OnEnable() {
        _Creator = target as PathCreator;

        if (_Creator.Path == null) {
            _Creator.CreatePath();
        }

        _Path = _Creator.Path;

        _EditModeButton = new GUIContent(
            EditorGUIUtility.IconContent("EditCollider").image,
            EditorGUIUtility.TrTextContent("Edit spline path.\n\n - Click while holding shift to add more segments.").text
        );
    }
}
