#region Definations


public enum NoneType {
    None,
}

public enum ChapterType {
    Spring,
    Summer,
    Fall,
    Winter,
}

public enum StageType {
    First,
    Second,
    Third,
    Fourth,
}

public enum CardRank {
    Normal,
    Rare,
    Unique,
    Legendary,
    Mystic,
}

public enum WeaponRank {
    Normal,
    Rare,
    Unique,
    Legendary
}

public enum ChestRank {
    Normal,
    Unique,
    Mystic
}

public enum ItemType {
    Guard,
    Key,
}

public enum StateType {
    Blind,
    Blood,
    BloodRage,
    Burn,
    ColdSnow,
    Confusion,
    Electric,
    Energy,
    FastHeal,
    FastSpeed,
    Fear,
    Fear2,
    HealGreen,
    Paralysis,
    Poison1,
    Poison2,
    PowerUp,
    Relieve,
    ShinyStar,
    Sleep,
    SlowDown,
    Stuned,
    SuckBlood,
    Rage
}

public enum RoomType {
    Undefined,
    Entrance,
    Normal,
    Hub,
    Reward,
    Shop,
    Boss,
    Exit,
    Secret
}

public enum RoomLinkType {
    Up,
    Down,
    Left,
    Right,
    Unknown
}

public enum RoomLinkDir {
    Horizontal,
    Vertical
}

public enum PressKey {
    Q, W, E, R, Space
}

public enum WeaponEssence {
    None,
    BloodSuck,
    Ice,
    Stun,
    Poision,
    AttFear,
    DefFear
}

#endregion Definations





#region Common class

public static class Common {
    public static int baseLevel = 1;
    public static int maxLevel = 30;
    public static int baseHp = 300;
    public static int baseMoveSpeed = 6;
    public static int maxMoveSpeed = 10;
    public static int baseAd = 30;
    public static int baseAp = 30;
    public static int baseDef = 10;
    public static int baseCritChance = 0;
    public static int baseCritDamage = 5;
    public static int baseFeverMaxGauge = 100;
    public static int secondFeverMaxGauge = 90;
    public static int thirdFeverMaxGauge = 80;
    public static float cardNormalRate = 0.49f;
    public static float cardRareRate = 0.35f;
    public static float cardUniqueRate = 0.14f;
    public static float cardLegendaryRate = 0.0196f;
    public static float cardMysticRate = 0.0004f;
}

#endregion // Common class