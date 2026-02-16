using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStat", menuName = "Scriptable Objects/WeaponStat")]
public class WeaponStat : ScriptableObject
{
    public GameObject weaponPrefab;

    public GameObject bullet;

    public bool isAcquired = false;

    public bool isHitscan = true;

    public bool isShotgun = false;

    public int pelletNum = 0;

    public float RateOfFire = 0f;

    public float range = 0f;

    public int Damage = 0;

    public float recoil = 0f;

    public bool IsAutomatic = false;

    public int magSize = 0;

}
