public class NoWeapon : WeaponBase {

    new private void Start() {
        SetEquipState(true);
        base.Start();
    }
    #region Garbage
    protected override void BasicAttackAnimation() {
    }
    protected override void Skill1Animation() {
    }
    protected override void Skill2Animation() {
    }
    protected override void FeverSkillAnimation() {
    }
    public override void BasicAttack() {
    }
    public override void Skill1() {
    }
    public override void Skill2() {
    }
    public override void FeverSkill() {
    }
    #endregion
}
