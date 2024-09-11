using UnityEngine;

public class MonsterBody : MonoBehaviour {
    private MonsterBase _monster;

    private void Start() {
        _monster = GetComponentInParent<MonsterBase>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        _monster.BodyDamage(collision);
    }
}
