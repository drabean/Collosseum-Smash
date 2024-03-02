using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconHolder : MonoBehaviour
{
    public List<Transform> Icons = new List<Transform>();

    float offset = 0.8f;
    public void addIcon(Transform icon)
    {
        Icons.Add(icon);
        icon.SetParent(transform);
        setIconOffsets();
    }

    public void removeIcon(Transform icon)
    {
        Icons.Remove(icon);
        setIconOffsets();
    }

    void setIconOffsets()
    {
        float start = (-1) * (offset / 2) * (Icons.Count - 1);

        for(int i = 0; i < Icons.Count; i++)
        {
            Icons[i].transform.localPosition = Vector3.right * (start + i * offset);
        }
    }
}
