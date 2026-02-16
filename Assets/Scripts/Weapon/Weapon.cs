using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] LayerMask InteractLayers;
    [SerializeField] Transform ProjectileSpawnPoint;
    [SerializeField] GameObject bullet;

    RaycastHit hit;
    private System.Collections.Generic.List<RaycastHit> pelletHits = new System.Collections.Generic.List<RaycastHit>();

    float raycastRange = 0f;
    bool isSingleBulletHit;

    Animator WpnClassAnimator;

    void Awake()
    {
        WpnClassAnimator = GetComponentInChildren<Animator>();
    }

    public void PlayIntroAnim()
    {
        WpnClassAnimator.SetBool("Intro", true);

    }

    public void ShootHitScan(float range)
    {

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, InteractLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("레이캐스트 충돌 지점: " + hit.point);
            isSingleBulletHit = true;
            raycastRange = range;
        }

        WpnClassAnimator.SetTrigger("Shoot");
    }

    public void ShootShotgun(float range, int pellet, float recoil)
    {
        pelletHits.Clear();

        for (int i = 0; i < pellet; i++)
        {
            Vector3 origin = transform.position;
            Vector3 spreadDirection = GetSpreadDirection(transform.forward, recoil);

            RaycastHit pelletHit;
            if (Physics.Raycast(origin, spreadDirection, out pelletHit, range, InteractLayers, QueryTriggerInteraction.Ignore))
            {
                pelletHits.Add(pelletHit);
                //Debug.Log($"펠렛 {i}번이 {hit.collider.name}에 맞았습니다.");
            }

        }
        
        WpnClassAnimator.SetTrigger("Shoot");

    }


    public void ChainsawAnimEnd()
    {
        WpnClassAnimator.SetTrigger("StabEnd");
        WpnClassAnimator.ResetTrigger("Shoot");
    }


    public void ShootProj()
    {
        Projectile newProj =
        Instantiate(bullet, ProjectileSpawnPoint.position, transform.rotation).GetComponent<Projectile>();
        WpnClassAnimator.SetTrigger("Shoot");
    }

    public Vector3 GetSpreadDirection(Vector3 direction,float maxSpreadAngle)
    {
        float xSpread = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        float ySpread = Random.Range(-maxSpreadAngle, maxSpreadAngle);

        Quaternion rotation = Quaternion.Euler(xSpread, ySpread, 0);
        return rotation * direction;

    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        //if (pelletHits.Count == 0) return; 

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        foreach (var hitInfo in pelletHits)
        {
        
            Gizmos.color = Color.red;

            Gizmos.DrawLine(origin, hitInfo.point);
            Gizmos.DrawSphere(hitInfo.point, 0.1f);
        
        }

        if (isSingleBulletHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, hit.point);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(hit.point, origin + direction * raycastRange);
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + direction * raycastRange);
        }
    }
}
