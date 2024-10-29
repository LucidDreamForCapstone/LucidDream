using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node {
    [HideInInspector] public List<Node> _children = new List<Node>();

    public override Node Clone() {
        CompositeNode node = Instantiate(this);
        node._children = _children.ConvertAll(c => c.Clone());
        return node;
    }
}