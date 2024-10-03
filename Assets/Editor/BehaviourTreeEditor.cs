using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow {
    BehaviourTreeView _treeView;
    InspectorView _inspectorView;
    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow() {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
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
        if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) {
            _treeView.PopulateView(tree);
        }
    }

    void OnNodeSelectionChanged(NodeView node) {
        _inspectorView.UpdateSelection(node);
    }
}
