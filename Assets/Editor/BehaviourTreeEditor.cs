using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow {
    BehaviourTreeView _treeView;
    InspectorView _inspectorView;
    Vector2 _mousePos;

    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow() {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line) {
        if (Selection.activeObject is BehaviourTree) {
            OpenWindow();
            return true;
        }
        return false;
    }

    private void OnGUI() {
        _mousePos = Event.current.mousePosition;
        _treeView._worldMousePos = _mousePos;
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        //StyleSheet
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviourTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnSelectionChange() {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (!tree) {
            if (Selection.activeGameObject) {
                //BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                MonsterBase monsterBase = Selection.activeGameObject.GetComponent<MonsterBase>();
                if (monsterBase) {
                    tree = monsterBase._tree;
                }
            }
        }

        if (Application.isPlaying) {
            if (tree) {
                _treeView.PopulateView(tree);
            }
        }
        else {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) {
                _treeView.PopulateView(tree);
            }
        }
    }

    private void OnEnable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj) {
        switch (obj) {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }
    private void OnEditorUpdate() {
        Repaint();
    }

    void OnNodeSelectionChanged(NodeView node) {
        _inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate() {
        _treeView?.UpdateNodeStates();
    }
}
