using UnityEngine;

public class MonsterTest : MonsterBase {


    #region mono funcs 

    #endregion //mono funcs



    #region private funcs

    private void Attack() {
        _playerScript.Damaged(_damage);
    }

    private void Move() {
        if (!_isDead) {
            transform.position = Vector2.Lerp(transform.position, _playerScript.transform.position, 0.1f * Time.deltaTime * _moveSpeed);
            if (transform.position.x - _playerScript.transform.position.x > 0)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
        }
    }

    protected override void AttackMove() {

    }

    #endregion //private funcs

}
