using System;

public class RandomNode : CompositeNode {
    protected override void OnStart() {
        int length = _children.Count;
        for (int i = 0; i < length; i++) {
            _children[i]._monster = _monster;
        }
    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        int randomN = UnityEngine.Random.Range(0, _children.Count);
        return _children[randomN].Update();
    }
}
