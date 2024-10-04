using UnityEngine;

public class ChaseNode : ActionNode {
    public float _searchDist;
    public float _attackDist;
    //Player _player;

    protected override void OnStart() {
        //_player = PlayerDataManager.Instance.Player;
    }

    protected override void OnStop() {
        _monster._rigid.velocity = Vector2.zero;
    }

    protected override State OnUpdate() {
        Vector2 monsterPos = _monster.transform.position;
        Vector2 playerPos = _monster._playerScript.transform.position;
        Vector2 moveVec = playerPos - monsterPos;
        _monster._rigid.velocity = moveVec.normalized * _monster._moveSpeed;
        if (moveVec.x < 0)
            _monster._spriteRenderer.flipX = true;
        else
            _monster._spriteRenderer.flipX = false;

        if (!CheckDist(monsterPos, playerPos, _searchDist)) {
            return State.Failure;
        }
        else if (CheckDist(monsterPos, playerPos, _attackDist)) {
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
}
