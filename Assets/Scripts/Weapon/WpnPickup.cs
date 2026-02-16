using UnityEngine;

public class WpnPickup : Pickups
{
    [SerializeField] WeaponStat wso;


    protected override void OnPickup(WeaponManager fWM)
    {
        fWM.WeaponAcquire(wso);

    }
}
