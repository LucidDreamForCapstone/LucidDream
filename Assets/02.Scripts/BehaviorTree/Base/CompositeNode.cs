using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CompositeNode : Node {
    [HideInInspector] public List<Node> _children = new List<Node>();

    public override Node Clone() {
        CompositeNode node = Instantiate(this);
        node._children = _children.ConvertAll(c => c.Clone());
        node._children.ForEach(c => c._parent = node);
        return node;
    }

    public void SortChild() {
        _children = _children.OrderBy(c => c._position.y).ToList();
    }
}