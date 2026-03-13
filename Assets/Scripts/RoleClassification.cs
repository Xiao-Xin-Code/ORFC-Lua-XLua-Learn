using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoleClassification : MonoBehaviour
{
	[SerializeField]
	TMP_Text name_Text;

	[SerializeField]
	Transform role_parent;


	public void SetName(string name)
	{
		name_Text.text = name;
	}

	public void SetRoleLocation(Transform tag)
	{
		tag.SetParent(role_parent);
	}
}
