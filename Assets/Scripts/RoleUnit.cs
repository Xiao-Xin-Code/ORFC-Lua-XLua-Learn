using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleUnit : MonoBehaviour,IItem
{
	[SerializeField]
	TMP_Text name_text;
	[SerializeField]
	Image icon;
	[SerializeField]
	RectTransform rectTransform;

	public int ID { get; private set; }

	public RectTransform RectTransform => rectTransform;

	public void SetID(int id)
	{
		ID = id;
	}


	public void SetIcon(Sprite sprite)
	{
		icon.sprite = sprite;
	}

	public void SetName(string name)
	{
		name_text.text = name;
		//rectTransform.sizeDelta = new Vector2(name_text.preferredWidth + 2 * boundary, 50);
	}

	public void UpdateItem(params object[] data)
	{
		if (data != null && data.Length > 0)
		{
			try
			{
				int id = (int)data[0];
				ID = id;
				SetName(LuaManager.Instance.GetRoleInfo(id).Get<string>("name"));
				
			}
			catch
			{

			}

		}
	}
}
