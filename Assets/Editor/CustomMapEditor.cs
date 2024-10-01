using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomMapEditor : EditorWindow {
    private Vector2 worldOffset;
    private Vector2 worldDrag;
    private float zoomScale = 1.0f;
    private List<RoomNode> nodes = new List<RoomNode>();
    private List<RoomLink> edges = new List<RoomLink>();
    private RoomNode selectedNode = null;//update by "on the mouse"
    private RoomNode targetNode = null; //update by "double click"
    private bool isDrawingLine = false;
    private bool isDragging = false;
    private double lastClickTime = 0f;
    private const float doubleClickTime = 0.3f;

    [MenuItem("Window/Custom Map Graph")]
    public static void ShowWindow() {
        GetWindow<CustomMapEditor>("Custom Map Graph");
    }

    private void OnEnable() {
        // �� ������ ������Ʈ�� ���� �̺�Ʈ ���
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable() {
        // â�� ���� �� ������Ʈ �̺�Ʈ�� ����
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate() {
        // Repaint�� ȣ���Ͽ� OnGUI�� ������ ȣ��
        Repaint();
    }

    private void OnGUI() {
        HandleZoom(Event.current);
        DrawGrid(20 * zoomScale, 0.2f, Color.gray);
        DrawGrid(100 * zoomScale, 0.5f, Color.white);
        ProcessEvents(Event.current);
        HandleMouseEvents();
        HandleKeyboardEvents();
        DrawNodes();
        DrawLinks();
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

        worldOffset += worldDrag * 0.5f;

        Vector3 newOffset = new Vector3(worldOffset.x % gridSpacing, worldOffset.y % gridSpacing, 0);

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
        worldDrag = Vector2.zero;

        if (e.type == EventType.MouseDrag && e.button == 2) {
            OnDrag(e.delta);
        }
    }

    private void OnDrag(Vector2 delta) {
        worldDrag = delta;
        GUI.changed = true;
    }

    private void HandleZoom(Event e) {
        if (e.type == EventType.ScrollWheel) {
            float zoomDelta = -e.delta.y * 0.05f;
            zoomScale += zoomDelta;
            zoomScale = Mathf.Clamp(zoomScale, 0.5f, 1.5f);
            e.Use();
        }
    }

    // ���콺�� ��� �߽��� �մ� �� �׸���
    private void DrawLineToMouse(Event e) {
        if (isDrawingLine) {
            Vector2 nodeCenter = targetNode._position * zoomScale + worldOffset + new Vector2(50 * zoomScale, 25 * zoomScale); // ����� �߽� ��ǥ ���
            Vector2 mousePos = e.mousePosition;

            Handles.BeginGUI();
            Handles.color = Color.red;
            Handles.DrawLine(nodeCenter, mousePos);
            Handles.EndGUI();
        }
    }

    private void HandleMouseEvents() {
        Event e = Event.current;
        UpdateNodeUnderMouse(e);

        if (e.type == EventType.MouseDown && e.button == 0) {
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
        }

        // CtrlŰ�� ���� �� �� �׸��⸦ �ߴ�
        if (!e.control && isDrawingLine) {
            if (selectedNode != null && selectedNode != targetNode) {
                ShowLinkMenu();
            }
            isDrawingLine = false;
        }

        // ��� �巡�� ó��
        if (e.type == EventType.MouseDrag && e.button == 0 && !isDrawingLine && selectedNode != null) {
            isDragging = true;
            Vector2 nodeDragOffset = new Vector2(50, 25) * zoomScale;
            selectedNode._position = (e.mousePosition - worldOffset) / zoomScale - nodeDragOffset;
        }

        // ���콺 ��ư�� ������ ���� ����
        if (e.type == EventType.MouseUp && e.button == 0) {
            isDragging = false;
        }
    }

    private void AddNode(Vector2 position) {
        Vector2 worldPosition = (position - worldOffset) / zoomScale;
        RoomNode newNode = RoomNode.Create(nodes.Count, worldPosition);
        nodes.Add(newNode);
        targetNode = newNode;
    }
    private void ShowLinkMenu() {//�׳� ���⿡ ���� �˾Ƽ� �������ִ°� �� �����ڳ�
        GenericMenu menu = new GenericMenu();
        if (!selectedNode.GetLinkState(RoomLinkType.Up))
            menu.AddItem(new GUIContent("LinkUp"), false, AddLink, RoomLinkType.Up);
        if (!selectedNode.GetLinkState(RoomLinkType.Down))
            menu.AddItem(new GUIContent("LinkDown"), false, AddLink, RoomLinkType.Down);
        if (!selectedNode.GetLinkState(RoomLinkType.Left))
            menu.AddItem(new GUIContent("LinkLeft"), false, AddLink, RoomLinkType.Left);
        if (!selectedNode.GetLinkState(RoomLinkType.Right))
            menu.AddItem(new GUIContent("LinkRight"), false, AddLink, RoomLinkType.Right);
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


    private void DrawNodes() {
        int length = nodes.Count;
        for (int i = 0; i < length; i++) {
            RoomNode node = nodes[i];
            Vector2 nodePos = node._position * zoomScale + worldOffset;
            float nodeWidth = 100 * zoomScale;
            float nodeHeight = 50 * zoomScale;

            // ���õ� ����� ���� ����
            if (node == targetNode) {
                GUI.color = Color.green;
            }
            else if (isDrawingLine && node == selectedNode) {
                GUI.color = Color.red;
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
        bool found = false;
        for (int i = 0; i < length; i++) {
            RoomNode node = nodes[i];
            Rect nodeRect = new Rect(node._position.x * zoomScale + worldOffset.x, node._position.y * zoomScale + worldOffset.y, 100 * zoomScale, 50 * zoomScale);
            if (nodeRect.Contains(e.mousePosition)) {
                if (!isDragging)
                    selectedNode = node;
                found = true;
                break;
            }
        }
        if (!found) {
            selectedNode = null;
        }
    }

    private void AddLink(object linkDir) {
        RoomLink newLink = RoomLink.Create(edges.Count, (RoomLinkDir)linkDir);
        newLink.SetLink(targetNode, selectedNode);
        edges.Add(newLink);
    }

    private void DrawLinks() {
        int length = edges.Count;
        for (int i = 0; i < length; i++) {
            RoomLink link = edges[i];
            Vector2 hostCenter = link._room1._position * zoomScale + worldOffset + new Vector2(50 * zoomScale, 25 * zoomScale);
            Vector2 targetCenter = link._room2._position * zoomScale + worldOffset + new Vector2(50 * zoomScale, 25 * zoomScale);
            Handles.BeginGUI();
            Handles.color = Color.green;
            Handles.DrawLine(hostCenter, targetCenter);
            Handles.EndGUI();
        }
    }
}
