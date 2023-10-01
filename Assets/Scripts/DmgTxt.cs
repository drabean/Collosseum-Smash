using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DmgTxt : MonoBehaviour
{
    [SerializeField] AnimationCurve moveX; // TMP ������ü�� X ��ǥ�� �̵����
    [SerializeField] AnimationCurve moveY;// TMP ������ü�� X ��ǥ�� �̵����

    [SerializeField] Poolable poolable;
    [SerializeField] Transform tmpTr; // ������Ʈ ���� TMP�� transform
    [SerializeField] TextMeshPro tmp; // TMP ������Ʈ ����

    [SerializeField] float showTime = 0.5f;    // ����ð�.
    [SerializeField] Vector3 offset;
    public void setColor(Color color)
    {
        tmp.color = color;
    }
    public void Show(string txt, Vector3 originPos)
    {
        tmp.text = txt;
        transform.position = originPos + offset;
        StartCoroutine(co_DmgTxt());
    }
    IEnumerator co_DmgTxt()
    {
        float xDir = Random.Range(-0.3f, 0.3f);

        float timeLeft = 0.0f;

        while(timeLeft <= showTime)
        {
            tmpTr.localPosition = xDir * moveX.Evaluate(timeLeft * (1/showTime)) * Vector3.right + moveY.Evaluate(timeLeft * (1 / showTime)) * Vector3.up;
            timeLeft += Time.deltaTime;
            yield return null;
        }
        poolable.Push();
        yield return null;
    }
}
