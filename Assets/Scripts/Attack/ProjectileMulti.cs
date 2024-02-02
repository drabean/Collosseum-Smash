using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMulti : Attack
{
    [SerializeField] Projectile projectile;
    public int ShootNum;
    public float Radian; // ���� ��Ȯ��
    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        Vector3 originVec = targetPos - startPos;
        float originRadian = Mathf.Atan2(originVec.y, originVec.x);

        //���⼭ �ǵ����
        //float radians = (45 + j * 90) * Mathf.Deg2Rad;

        for (int i = 0; i < ShootNum; i++)
        {
            //���۰���
            float radians = originRadian + Radian * (i - ((float)ShootNum / 2 - 0.5f)) * Mathf.Deg2Rad;

            Instantiate(projectile).Shoot(startPos, startPos + Vector3.right * Mathf.Cos(radians) + Vector3.up * Mathf.Sin(radians));
        }
    }

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time)
    {
        Vector3 originVec = targetPos - startPos;
        float originRadian = Mathf.Atan2(originVec.y, originVec.x);
       // float originRadian = 
        //���⼭ �ǵ����
        //float radians = (45 + j * 90) * Mathf.Deg2Rad;

        for (int i = 0; i < ShootNum; i++)
        {
            //���۰���
            float radians = originRadian + Radian * (i - ((float)ShootNum / 2 - 0.5f)) * Mathf.Deg2Rad;

            projectile.ShowWarning(startPos, startPos + Vector3.right * Mathf.Cos(radians) + Vector3.up * Mathf.Sin(radians), time);
        }

        return null;
    }
}
