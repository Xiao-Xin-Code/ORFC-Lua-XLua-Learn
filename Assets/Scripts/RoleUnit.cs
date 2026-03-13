using TMPro;
using UnityEngine;

public class RoleUnit : MonoBehaviour
{
	[SerializeField]
	TMP_Text name_text;

	public int ID { get; private set; }

	public void SetID(int id)
	{
		ID = id;
	}


	public void SetName(string name)
	{
		name_text.text = name;
	}
}
