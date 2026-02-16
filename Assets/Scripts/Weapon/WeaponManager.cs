using StarterAssets;
using TMPro;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    //[SerializeField] TMP_Text ammoTxt;

    [SerializeField] WeaponStat Fist;
    [SerializeField] WeaponStat Pistol;
    [SerializeField] WeaponStat Shotgun;
    [SerializeField] WeaponStat SSG;
    [SerializeField] WeaponStat Chaingun;
    [SerializeField] WeaponStat RL;
    [SerializeField] WeaponStat Plasma;
    [SerializeField] WeaponStat Chainsaw;


    StarterAssetsInputs SAInput;
    FirstPersonController FPController;

    Animator animator;

    WeaponStat currWpnSO;

    Weapon CurrWeap;

    int currAmmo = 0;

    float timeSineLastShot = 0f;

    bool isChainsawStabbing = false;


    void Awake()
    {
        SAInput = GetComponentInParent<StarterAssetsInputs>();
        FPController = GetComponentInParent<FirstPersonController>();
        animator = GetComponent<Animator>();

    }

    void Start()
    {
        SwitchWeapon(Fist);
        AdjustAmmo(currWpnSO.magSize);
    }

    void Update()
    {
        HandleWpnSwitch();
        HandleShoot();
    }

    public void AdjustAmmo(int value)
    {
        currAmmo += value;

        if (currAmmo > currWpnSO.magSize)
            currAmmo = currWpnSO.magSize;

    }

    public void HandleShoot()
    {
        timeSineLastShot += Time.deltaTime;

        if (!SAInput.shoot)
        {
            if (currWpnSO == Chainsaw && isChainsawStabbing)
            {
                CurrWeap.ChainsawAnimEnd();
            }
            else return;
        }

        if (timeSineLastShot >= currWpnSO.RateOfFire)
        {
            if (currWpnSO.isHitscan) CurrWeap.ShootHitScan(currWpnSO.range);
            else if (currWpnSO.isShotgun) CurrWeap.ShootShotgun(currWpnSO.range, currWpnSO.pelletNum, currWpnSO.recoil);
            else CurrWeap.ShootProj();

            //AdjustAmmo(-1);

            timeSineLastShot = 0.0f;
        }

        if (currWpnSO == Chainsaw) isChainsawStabbing = SAInput.shoot;

        if (!currWpnSO.IsAutomatic) SAInput.ShootInput(false);

    }

    public void HandleWpnSwitch()
    {

        if (SAInput.switchFist && Fist.isAcquired)
        {
            SwitchWeapon(Fist);
            SAInput.switchFist = false;
        }
        else if (SAInput.switchPistol)
        {
            if(Pistol.isAcquired) SwitchWeapon(Pistol);
            SAInput.switchPistol = false;
        }

        else if (SAInput.switchShotgun)
        {
            if (Shotgun.isAcquired) SwitchWeapon(Shotgun);
            SAInput.switchShotgun = false;
        }


        else if (SAInput.switchSSG)
        {
            if(SSG.isAcquired) SwitchWeapon(SSG);
            SAInput.switchSSG = false;
        }

        else if (SAInput.switchRocket)
        {
            if(RL.isAcquired) SwitchWeapon(RL);
            SAInput.switchRocket = false;
        }

        else if (SAInput.switchPlasma)
        {
            if(Plasma.isAcquired) SwitchWeapon(Plasma);
            SAInput.switchPlasma = false;
        }

        else if (SAInput.switchChaingun)
        {
            if(Chaingun.isAcquired) SwitchWeapon(Chaingun);
            SAInput.switchChaingun = false;
        }

        else if (SAInput.switchChainsaw)
        {
            if(Chainsaw.isAcquired) SwitchWeapon(Chainsaw);
            SAInput.switchChainsaw = false;
        }



    }

    public void WeaponAcquire(WeaponStat fWS)
    {
        if (!fWS.isAcquired) fWS.isAcquired = true;
        SwitchWeapon(fWS);

        CurrWeap.PlayIntroAnim();

    }

    public void SwitchWeapon(WeaponStat fWS)
    {
        //Debug.Log(fWS.name);

        if (CurrWeap)
        {
            Destroy(CurrWeap.gameObject);
        }

        Weapon newWeap = Instantiate(fWS.weaponPrefab, transform).GetComponent<Weapon>();

        CurrWeap = newWeap;
        this.currWpnSO = fWS;
        AdjustAmmo(fWS.magSize);

    }

    
    



}
