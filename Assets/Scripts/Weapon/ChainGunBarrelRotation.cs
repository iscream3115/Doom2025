using UnityEngine;
using StarterAssets;

public class ChainGunBarrelRotation : MonoBehaviour
{
    StarterAssetsInputs SAI;

    public float maxRotationSpeed = 3000f; 

    public float acceleration = 500f; 
    public float deceleration = 1000f;

    private float currentRotationSpeed = 0f;

    void Awake()
    {
        SAI = GetComponentInParent<StarterAssetsInputs>();
    }

    void Update()
    {
        if(SAI.shoot)
        {
            currentRotationSpeed = Mathf.MoveTowards(
                currentRotationSpeed, 
                maxRotationSpeed, 
                acceleration * Time.deltaTime);
        }
        else
        {
            currentRotationSpeed = Mathf.MoveTowards(
                currentRotationSpeed, 
                0f, 
                deceleration * Time.deltaTime
            );
        }

        if (currentRotationSpeed > 0f)
        {
            this.transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime, Space.Self);
        }
    }


}
