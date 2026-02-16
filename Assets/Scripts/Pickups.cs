using UnityEngine;

public abstract class Pickups : MonoBehaviour
{
    [SerializeField] float RotationSpeed = 100f;
    const string PLAYER_STRING = "Player";


    void Update()
    {
        transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_STRING))
        {
            WeaponManager WM = other.GetComponentInChildren<WeaponManager>();
            OnPickup(WM);
            Destroy(this.gameObject);
        }


    }

    protected abstract void OnPickup(WeaponManager fWM);
}
