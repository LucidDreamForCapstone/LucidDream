using UnityEngine;

public class ChaseNode : ActionNode {
    //Player _player;

    protected override void OnStart() {
        //_player = PlayerDataManager.Instance.Player;
    }

    protected override void OnStop() {
        _monster._rigid.velocity = Vector2.zero;
        if (CheckAnimatorParamExist("Chase", AnimatorControllerParameterType.Bool)) {
            _monster._animator.SetBool("Chase", false);
        }
    }

    protected override State OnUpdate() {
        Vector2 monsterPos = _monster.transform.position;
        Vector2 playerPos = _monster._playerScript.transform.position;
        Vector2 moveVec = playerPos - monsterPos;
        _monster._rigid.velocity = moveVec.normalized * _monster._moveSpeed;

        if (CheckAnimatorParamExist("Chase", AnimatorControllerParameterType.Bool)) {
            _monster._animator.SetBool("Chase", true);
        }

        if (moveVec.x < 0)
            _monster._spriteRenderer.flipX = _monster._isRightDefault;
        else
            _monster._spriteRenderer.flipX = !_monster._isRightDefault;

        if (!CheckDist(monsterPos, playerPos, _monster._searchDist)) {
            return State.Failure;
        }
        else if (CheckDist(monsterPos, playerPos, _monster._attackDist)) {
            return State.Success;
        }
        return State.Running;
    }

    //Check if distance of {a, b} is less than targetDist
    bool CheckDist(Vector2 a, Vector2 b, float targetDist) {
        float distSquare = (a - b).sqrMagnitude;
        if (distSquare < targetDist * targetDist) {
            return true;
        }
        return false;
    }

    bool CheckAnimatorParamExist(string name, AnimatorControllerParameterType type) {
        // Animator의 모든 파라미터를 가져옴
        foreach (var param in _monster._animator.parameters) {
            // 파라미터 이름이 일치하고, 트리거 타입일 경우 실행
            if (param.name == name && param.type == type) {
                return true;
            }
        }
        return false;
    }
}
