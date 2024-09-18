using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomMapEditor : EditorWindow {
    private Vector2 offset;
    private Vector2 drag;
    private float zoomScale = 1.0f;
    private List<RoomNode> nodes = new List<RoomNode>();
    private List<RoomLink> edges = new List<RoomLink>();
    private RoomNode selectedNode = null;//update by "on the mouse"
    private RoomNode targetNode = null; //update by "double click"
    private Vector2 nodeDragOffset;
    private bool isDrawingLine = false;

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
        HandleKeyboardEvents();
        DrawNodes();
        DrawLineToMouse(Event.current);

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

    // ���콺�� ��� �߽��� �մ� �� �׸���
    private void DrawLineToMouse(Event e) {
        if (isDrawingLine && selectedNode != null) {
            Vector2 nodeCenter = selectedNode._position * zoomScale + offset + new Vector2(50 * zoomScale, 25 * zoomScale); // ����� �߽� ��ǥ ���
            Vector2 mousePos = e.mousePosition;

            Handles.BeginGUI();
            Handles.color = Color.red;
            Handles.DrawLine(nodeCenter, mousePos);
            Handles.EndGUI();
        }
    }

    private void HandleMouseEvents() {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) {
            UpdateNodeUnderMouse(e);

            // ���� Ŭ�� ����
            double currentTime = EditorApplication.timeSinceStartup;
            if (currentTime - lastClickTime < doubleClickTime) {
                if (selectedNode != null) {
                    // ��� ���� �ν����� â�� ǥ��
                    Selection.activeObject = selectedNode;
                    targetNode = selectedNode;
                }
                else {
                    AddNode(e.mousePosition);
                }
            }
            lastClickTime = currentTime;
        }

        if (e.type == EventType.MouseDown && e.button == 0 && e.control && targetNode != null) {
            isDrawingLine = true;
            Repaint();
        }

        // CtrlŰ�� ���� �� �� �׸��⸦ �ߴ�
        if (!e.control && isDrawingLine) {
            isDrawingLine = false;
            Repaint();
        }

        // ��Ŭ�� ���� (���ؽ�Ʈ �޴�)
        if (e.type == EventType.MouseDown && e.button == 1) {
            int length = nodes.Count;
            for (int i = 0; i < length; i++) {
                RoomNode node = nodes[i];
                Rect nodeRect = new Rect(node._position.x * zoomScale + offset.x, node._position.y * zoomScale + offset.y, 100 * zoomScale, 50 * zoomScale);
                if (nodeRect.Contains(e.mousePosition) && targetNode == node) {
                    selectedNode = node;
                    ShowContextMenu();  // ��Ŭ�� �� ���ؽ�Ʈ �޴� ǥ��
                    break;
                }
            }
        }

        // ��� �巡�� ó��
        if (e.type == EventType.MouseDrag && e.button == 0 && !e.control && selectedNode != null) {
            selectedNode._position = (e.mousePosition + nodeDragOffset - offset) / zoomScale;
            GUI.changed = true;
        }

        // ���콺 ��ư�� ������ ���� ����
        if (e.type == EventType.MouseUp && e.button == 0) {
            selectedNode = null;
        }
    }

    private void AddNode(Vector2 position) {
        Vector2 worldPosition = (position - offset) / zoomScale;
        RoomNode newNode = RoomNode.CreateInstance(nodes.Count, worldPosition);
        nodes.Add(newNode);
        targetNode = newNode;
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
        menu.ShowAsContext();
    }

    private void HandleKeyboardEvents() {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete) {
            if (targetNode != null) {
                DeleteNode();
                targetNode = null;
                e.Use();
            }
        }
    }

    private void DeleteNode() {
        if (targetNode != null) {
            nodes.Remove(targetNode);
            targetNode = null;
            GUI.changed = true;
        }
    }

    private void FindLinkTarget(object linkType) {
        RoomLinkType roomLinkType = (RoomLinkType)linkType;

    }

    private void DrawNodes() {
        int length = nodes.Count;
        for (int i = 0; i < length; i++) {
            RoomNode node = nodes[i];
            Vector2 nodePos = node._position * zoomScale + offset;
            float nodeWidth = 100 * zoomScale;
            float nodeHeight = 50 * zoomScale;

            // ���õ� ����� ���� ����
            if (node == targetNode) {
                GUI.color = Color.green;
            }
            else {
                GUI.color = Color.white;
            }


            GUIStyle style = new GUIStyle(GUI.skin.button) {
                fontSize = Mathf.RoundToInt(12 * zoomScale),
                alignment = TextAnchor.MiddleCenter
            };

            node.UpdateRoomName();
            GUI.Label(new Rect(nodePos.x, nodePos.y, nodeWidth, nodeHeight), node._roomName, style);
        }
    }

    // ���콺�� �ִ� ��ġ�� �ִ� ��带 ��ȯ�ϴ� �Լ�
    private void UpdateNodeUnderMouse(Event e) {
        int length = nodes.Count;
        for (int i = 0; i < length; i++) {
            RoomNode node = nodes[i];
            Rect nodeRect = new Rect(node._position.x * zoomScale + offset.x, node._position.y * zoomScale + offset.y, 100 * zoomScale, 50 * zoomScale);
            if (nodeRect.Contains(e.mousePosition)) {
                selectedNode = node;
                nodeDragOffset = node._position * zoomScale + offset - e.mousePosition;
                break;
            }
        }
    }
}
