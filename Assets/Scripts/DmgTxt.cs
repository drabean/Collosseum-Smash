using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DmgTxt : MonoBehaviour
{
    [SerializeField] AnimationCurve moveX; // TMP 하위객체의 X 좌표계 이동경로
    [SerializeField] AnimationCurve moveY;// TMP 하위객체의 X 좌표계 이동경로

    [SerializeField] Poolable poolable;
    [SerializeField] Transform tmpTr; // 오브젝트 하위 TMP의 transform
    [SerializeField] TextMeshPro tmp; // TMP 컴포넌트 참조

    [SerializeField] float showTime = 0.5f;    // 노출시간.
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
