using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float radius = 2.0f;
    //[SerializeField] int dmg = 3;

    const string PLAYER_STRING = "Player";


    void Start()
    {
        Explode();
    }


    public void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hc in hitColliders)
        {
            //폭발에 휩쓸릴 경우 작성하는 스크립트



        }


    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
