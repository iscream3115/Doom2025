using UnityEngine;

[CreateAssetMenu(fileName = "EnemySOManager", menuName = "Scriptable Objects/EnemySOManager")]
public class EnemySOManager : ScriptableObject
{

    public GameObject Projectile;
    public float HP;
    public float MeleeDMG;
    public float MeleeRange;
    public bool isHitScan = false;

    public float ViewAngle;
    public float ViewDistance;
    public float HearingRange;

    public float MoveDuration;
    public float IdleDuration;
    public Vector2 MoveDurationRange;
    public Vector2 IdleDurationRange;

    
    
}
