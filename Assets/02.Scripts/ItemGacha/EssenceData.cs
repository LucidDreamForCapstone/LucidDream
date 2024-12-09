using System;
using UnityEngine;

[Serializable]
public class EssenceData {
    public WeaponEssence type;
    public CardRank rank;
    public float chance;
    public float lastTime;
    public float ratio;

    [Header("\nPoison")]
    public int tickDamage;
    public float tickTime;
    public int tickCount;

    public EssenceData(WeaponEssence type, CardRank rank, float chance, float lastTime, float ratio, int tickDamage, float tickTime, int tickCount) {
        this.type = type;
        this.rank = rank;
        this.chance = chance;
        this.lastTime = lastTime;
        this.ratio = ratio;
        this.tickDamage = tickDamage;
        this.tickTime = tickTime;
        this.tickCount = tickCount;
    }
}
