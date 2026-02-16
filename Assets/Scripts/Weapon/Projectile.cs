using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int Dmg;
    [SerializeField] float Speed = 10f;
    [SerializeField] GameObject ProjParticle;

    Rigidbody rb;
    GameObject Player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (rb)
            rb.linearVelocity = this.transform.forward * Speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(ProjParticle, this.transform.position, this.transform.rotation);

        Destroy(this.gameObject);
    }
}
