using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Attack ShooterProjectile;

    public float ShootTime;
    float shootTimeLeft;

    public LayerMask layer;


    public GameObject ShooterPrefab;
    public ParticleSystem Particle;
    private void Start()
    {
        StartCoroutine(co_ShootMagicMissile());
    }
    IEnumerator co_ShootMagicMissile()
    {
        shootTimeLeft = ShootTime;
        while (true)
        {
            shootTimeLeft -= Time.deltaTime;

            if (shootTimeLeft <= 0)
            {
                shootTimeLeft = ShootTime;

                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 7.0f, Vector3.forward, 0f, layer);

                if (hits.Length == 0) continue;
                Vector3 startPos = transform.position;
                Vector3 targetPosition = hits[0].point;

                Destroy(Instantiate(ShooterPrefab, startPos, Quaternion.identity), 0.5f);
                ShooterProjectile.ShowWarning(startPos, targetPosition, 0.5f);
                yield return new WaitForSeconds(0.5f);
                Destroy(Instantiate(Particle, startPos, Quaternion.identity), 0.5f);
                Instantiate(ShooterProjectile).Shoot(startPos,targetPosition);

            }
            yield return null;
        }
    }

}
