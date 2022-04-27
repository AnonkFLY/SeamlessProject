[System.Serializable]
public class Damage
{
    public float ordinaryDamage;
    public float trueDamage;
    public Player damageOrigin;

    public Damage(float ordinaryDamage, float trueDamage, Player damageOrigin)
    {
        this.ordinaryDamage = ordinaryDamage;
        this.trueDamage = trueDamage;
        this.damageOrigin = damageOrigin;
    }
}