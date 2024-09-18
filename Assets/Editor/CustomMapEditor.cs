using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomMapEditor : EditorWindow {
    private Vector2 offset;
    private Vector2 drag;
    private float zoomScale = 1.0f;
    private List<RoomNode> nodes = new List<RoomNode>();

    private RoomNode selectedNode = null;
    private Vector2 nodeDragOffset;

    private double lastClickTime = 0f;
    private const float doubleClickTime = 0.3f;

    [MenuItem("Window/Custom Map Graph")]
    public static void ShowWindow() {
        GetWindow<CustomMapEditor>("Custom Map Graph");
    }

    private void OnGUI() {
        HandleZoom(Event.current);
        DrawGrid(20 * zoomScale, 0.2f, Color.gray);
        DrawGrid(100 * zoomScale, 0.5f, Color.white);
        ProcessEvents(Event.current);
        HandleMouseEvents();
        DrawNodes();

        if (GUI.changed) {
            Repaint();
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Color originalColor = Handles.color;
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;

        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++) {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing) + newOffset,
                             new Vector3(gridSpacing * i, position.height) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++) {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                             new Vector3(position.width, gridSpacing * j, 0) + newOffset);
        }

        Handles.color = originalColor;
        Handles.EndGUI();
    }

    private void ProcessEvents(Event e) {
        drag = Vector2.zero;

        if (e.type == EventType.MouseDrag && e.button == 2) {
            OnDrag(e.delta);
        }
    }

    private void OnDrag(Vector2 delta) {
        drag = delta;
        GUI.changed = true;
    }

    private void HandleZoom(Event e) {
        if (e.type == EventType.ScrollWheel) {
            float zoomDelta = -e.delta.y * 0.05f;
            zoomScale += zoomDelta;
            zoomScale = Mathf.Clamp(zoomScale, 0.5f, 3f);
            e.Use();
        }
    }

    private void HandleMouseEvents() {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) {
            Vector2 mousePos = (e.mousePosition - offset) / zoomScale;

            // 노드 선택 및 드래그 준비
            foreach (var node in nodes) {
                Rect nodeRect = new Rect(node._position.x * zoomScale + offset.x, node._position.y * zoomScale + offset.y, 100 * zoomScale, 50 * zoomScale);
                if (nodeRect.Contains(e.mousePosition)) {
                    selectedNode = node;
                    nodeDragOffset = (Vector2)node._position * zoomScale + offset - e.mousePosition;
                    break;
                }
            }

            // 더블 클릭 감지
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - lastClickTime < doubleClickTime) {
                if (selectedNode != null) {
                    // 노드 정보 인스펙터 창에 표시
                    Selection.activeObject = selectedNode;
                }
                else {
                    AddNode(e.mousePosition);
                }
            }
            lastClickTime = currentTime;
        }

        // 우클릭 감지 (컨텍스트 메뉴)
        if (e.type == EventType.MouseDown && e.button == 1) {
            Vector2 mousePos = (e.mousePosition - offset) / zoomScale;
            foreach (var node in nodes) {
                Rect nodeRect = new Rect(node._position.x * zoomScale + offset.x, node._position.y * zoomScale + offset.y, 100 * zoomScale, 50 * zoomScale);
                if (nodeRect.Contains(e.mousePosition)) {
                    selectedNode = node;
                    ShowContextMenu();  // 우클릭 시 컨텍스트 메뉴 표시
                    break;
                }
            }
        }

        // 노드 드래그 처리
        if (e.type == EventType.MouseDrag && e.button == 0 && selectedNode != null) {
            selectedNode._position = (e.mousePosition + nodeDragOffset - offset) / zoomScale;
            GUI.changed = true;
        }

        // 마우스 버튼을 놓으면 선택 해제
        if (e.type == EventType.MouseUp && e.button == 0) {
            selectedNode = null;
        }
    }

    private void AddNode(Vector2 position) {
        Vector2 worldPosition = (position - offset) / zoomScale;
        RoomNode newNode = RoomNode.CreateInstance(nodes.Count, worldPosition);
        nodes.Add(newNode);
        Repaint();
    }
    private void ShowContextMenu() {
        GenericMenu menu = new GenericMenu();
        if (!selectedNode.GetLinkState(RoomLinkType.Up))
            menu.AddItem(new GUIContent("LinkUp"), false, FindLinkTarget, RoomLinkType.Up);
        if (!selectedNode.GetLinkState(RoomLinkType.Down))
            menu.AddItem(new GUIContent("LinkDown"), false, FindLinkTarget, RoomLinkType.Down);
        if (!selectedNode.GetLinkState(RoomLinkType.Left))
            menu.AddItem(new GUIContent("LinkLeft"), false, FindLinkTarget, RoomLinkType.Left);
        if (!selectedNode.GetLinkState(RoomLinkType.Right))
            menu.AddItem(new GUIContent("LinkRight"), false, FindLinkTarget, RoomLinkType.Right);
        menu.AddItem(new GUIContent("Delete"), false, DeleteNode);  // "Delete" 메뉴 항목 추가
        menu.ShowAsContext();
    }

    private void DeleteNode() {
        if (selectedNode != null) {
            nodes.Remove(selectedNode);
            selectedNode = null;
            GUI.changed = true;
        }
    }

    private void FindLinkTarget(object linkType) {
        RoomLinkType roomLinkType = (RoomLinkType)linkType;

    }

    private void DrawNodes() {
        foreach (var node in nodes) {
            Vector2 nodePos = node._position * zoomScale + offset;
            float nodeWidth = 100 * zoomScale;
            float nodeHeight = 50 * zoomScale;
            GUIStyle style = new GUIStyle(GUI.skin.button) {
                fontSize = Mathf.RoundToInt(12 * zoomScale),
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Label(new Rect(nodePos.x, nodePos.y, nodeWidth, nodeHeight), node._roomName, style);
        }
    }
}
