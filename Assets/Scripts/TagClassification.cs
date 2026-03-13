using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TagClassification : MonoBehaviour
{
    [SerializeField]
    TMP_Text name_Text;

    [SerializeField]
    Transform tag_parent;


    public void SetName(string name)
    {
        name_Text.text = name;
    }

    public void SetTagLocation(Transform tag)
    {
        tag.SetParent(tag_parent);
    }
}
