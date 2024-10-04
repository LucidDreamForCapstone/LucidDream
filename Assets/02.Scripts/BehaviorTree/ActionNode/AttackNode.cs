public class AttackNode : ActionNode {
    protected override void OnStart() {

    }

    protected override void OnStop() {

    }

    protected override State OnUpdate() {
        switch (_monster._attackState) {
            case MonsterBase.AttackState.Ready:
                _monster.Attack().Forget();
                return State.Running;
            case MonsterBase.AttackState.Attacking:
                return State.Running;
            case MonsterBase.AttackState.CoolTime:
                return State.Failure;
        }
        return State.Failure;
    }
}
