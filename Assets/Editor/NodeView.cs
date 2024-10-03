using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node {
    public Action<NodeView> OnNodeSelected;
    public Node _node;
    public Port _input;
    public Port _output;
    public NodeView(Node node) : base("Assets/Editor/NodeView.uxml") {
        _node = node;
        title = node.name;
        viewDataKey = node._guid;

        style.left = node._position.x;
        style.top = node._position.y;
        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetupClasses() {
        if (_node is ActionNode) {
            AddToClassList("action");
        }
        else if (_node is CompositeNode) {
            AddToClassList("composite");
        }
        else if (_node is DecoratorNode) {
            AddToClassList("decorator");
        }
        else if (_node is RootNode) {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts() {
        if (_node is ActionNode) {
            _input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (_node is CompositeNode) {
            _input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (_node is DecoratorNode) {
            _input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (_node is RootNode) {

        }

        if (_input != null) {
            _input.portName = "";
            _input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(_input);
        }
    }

    private void CreateOutputPorts() {
        if (_node is ActionNode) {

        }
        else if (_node is CompositeNode) {
            _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (_node is DecoratorNode) {
            _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (_node is RootNode) {
            _output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (_output != null) {
            _output.portName = "";
            _output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(_output);
        }
    }

    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        Undo.RecordObject(_node, "Behaviour Tree (Set Position)");
        _node._position.x = newPos.xMin;
        _node._position.y = newPos.yMin;
        EditorUtility.SetDirty(_node);
    }

    public override void OnSelected() {
        base.OnSelected();
        if (OnNodeSelected != null) {
            OnNodeSelected.Invoke(this);
        }
    }

    public void SortChildren() {
        CompositeNode composite = _node as CompositeNode;
        if (composite) {
            composite._children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(Node left, Node right) {
        return left._position.x < right._position.x ? -1 : 1;
    }

    public void UpdateState() {

        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying) {
            switch (_node._state) {
                case Node.State.Running:
                    if (_node._started) {
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
