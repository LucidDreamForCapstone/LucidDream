using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
public class BehaviourTreeView : GraphView {
    public Action<NodeView> OnNodeSelected;
    public Vector2 _worldMousePos;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    BehaviourTree _tree;
    public BehaviourTreeView() {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo() {
        PopulateView(_tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node) {
        return GetNodeByGuid(node._guid) as NodeView;
    }


    internal void PopulateView(BehaviourTree tree) {
        _tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree._rootNode == null) {
            tree._rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // Creates node view
        tree._nodes.ForEach(n => CreateNodeView(n));

        //Create edges
        tree._nodes.ForEach(n => {
            var children = tree.GetChildren(n);
            children.ForEach(c => {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView._output.ConnectTo(childView._input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
        if (graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach(elem => {
                NodeView nodeView = elem as NodeView;
                if (nodeView != null) {
                    _tree.DeleteNode(nodeView._node);
                }

                Edge edge = elem as Edge;
                if (edge != null) {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _tree.RemoveChild(parentView._node, childView._node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach(edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                _tree.AddChild(parentView._node, childView._node);
            });
        }

        if (graphViewChange.movedElements != null) {
            nodes.ForEach((n) => {
                NodeView view = n as NodeView;
                view.SortChildren();
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        //base.BuildContextualMenu(evt);
        evt.menu.AppendAction("Delete", (a) => DeleteSelection());
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }
    }

    void CreateNode(Type type) {
        Node node = _tree.CreateNode(type);
        Vector2 localMousePos = contentViewContainer.WorldToLocal(_worldMousePos);
        Vector2 offset = new Vector2(71, 24) * -1;
        node._position = localMousePos + offset;
        CreateNodeView(node);
    }

    void CreateNodeView(Node node) {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeStates() {
        nodes.ForEach(n => {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }
}
