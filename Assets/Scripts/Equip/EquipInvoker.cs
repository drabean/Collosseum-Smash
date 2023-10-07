using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipInvoker : MonoBehaviour
{
    Equip e;
    private void Awake()
    {
        e = GetComponent<Equip>();
    }

    private IEnumerator  Start()
    {
        yield return new WaitForSeconds(1.0f);
        e.onEquip(GameObject.FindObjectOfType<Player>());
    }
}
