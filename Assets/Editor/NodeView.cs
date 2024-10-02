using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView : UnityEditor.Experimental.GraphView.Node {
    public Node _node;
    public Port _input;
    public Port _output;
    public NodeView(Node node) {
        _node = node;
        title = node.name;
        viewDataKey = node._guid;

        style.left = node._position.x;
        style.top = node._position.y;

        CreateInputPorts();
        CreateOutputPorts();
    }

    private void CreateInputPorts() {
        if (_node is ActionNode) {
            _input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (_node is CompositeNode) {
            _input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (_node is DecoratorNode) {
            _input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }

        if (_input != null) {
            _input.portName = "";
            inputContainer.Add(_input);
        }
    }

    private void CreateOutputPorts() {
        if (_node is ActionNode) {

        }
        else if (_node is CompositeNode) {
            _output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (_node is DecoratorNode) {
            _output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (_output != null) {
            _output.portName = "";
            outputContainer.Add(_output);
        }
    }

    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        _node._position.x = newPos.xMin;
        _node._position.y = newPos.yMin;
    }
}
