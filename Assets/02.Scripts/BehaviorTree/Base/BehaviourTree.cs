using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject {
    public Node _rootNode;
    public List<Node> _nodes = new List<Node>();
    [HideInInspector] public Node.State _treeState = Node.State.Running;
    [HideInInspector] public MonsterBase _monster;

    public Node.State Update() {
        if (_rootNode._state == Node.State.Running) {
            _rootNode._monster = _monster;
            _treeState = _rootNode.Update();
        }
        return _treeState;
    }

#if UNITY_EDITOR
    public Node CreateNode(System.Type type) {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node._guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        _nodes.Add(node);

        if (!Application.isPlaying) {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node) {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        _nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child) {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator) {
            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
            decorator._child = child;
            EditorUtility.SetDirty(decorator);
        }

        RootNode root = parent as RootNode;
        if (root) {
            Undo.RecordObject(root, "Behaviour Tree (AddChild)");
            root._child = child;
            EditorUtility.SetDirty(root);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
            composite._children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public void RemoveChild(Node parent, Node child) {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator) {
            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
            decorator._child = null;
            EditorUtility.SetDirty(decorator);
        }

        RootNode root = parent as RootNode;
        if (root) {
            Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");
            root._child = null;
            EditorUtility.SetDirty(root);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite._children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }


#endif
    public List<Node> GetChildren(Node parent) {
        List<Node> children = new List<Node>();

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator._child != null) {
            children.Add(decorator._child);
        }

        RootNode root = parent as RootNode;
        if (root && root._child != null) {
            children.Add(root._child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            return composite._children;
        }

        return children;
    }
    public void Traverse(Node node, System.Action<Node> visiter) {
        if (node) {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public BehaviourTree Clone() {
        BehaviourTree tree = Instantiate(this);
        tree._rootNode = tree._rootNode.Clone();
        tree._nodes = new List<Node>();
        Traverse(tree._rootNode, (n) => {
            tree._nodes.Add(n);
        });

        return tree;
    }
}
