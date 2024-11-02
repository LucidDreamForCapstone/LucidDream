public class AttackNode : ActionNode {
    public int attackIndex;
    protected override void OnStart() {

    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        switch (_monster._attackStateList[attackIndex]) {
            case MonsterBase.AttackState.Ready:
                _monster._attackFuncList[attackIndex]?.Invoke().Forget();
                return State.Running;
            case MonsterBase.AttackState.Attacking:
                return State.Success;
            case MonsterBase.AttackState.CoolTime:
                return State.Failure;
        }
        return State.Failure;
    }
}
