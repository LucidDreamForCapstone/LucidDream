using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject {
    public Node _rootNode;
    public Node.State _treeState = Node.State.Running;
    public List<Node> _nodes = new List<Node>();

    public Node.State Update() {
        if (_rootNode._state == Node.State.Running) {
            _treeState = _rootNode.Update();
        }
        return _treeState;
    }

    public Node CreateNode(System.Type type) {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node._guid = GUID.Generate().ToString();
        _nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node) {
        _nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child) {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator) {
            decorator._child = child;
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            composite._children.Add(child);
        }
    }

    public void RemoveChild(Node parent, Node child) {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator) {
            decorator._child = null;
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            composite._children.Remove(child);
        }
    }

    public List<Node> GetChildren(Node parent) {
        List<Node> children = new List<Node>();

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator._child != null) {
            children.Add(decorator._child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            return composite._children;
        }

        return children;
    }
}
