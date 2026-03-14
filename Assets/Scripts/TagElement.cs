using TMPro;
using UnityEngine;

public class TagElement : MonoBehaviour,IItem
{
	float boundary = 10;

    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    TMP_Text name_text;


	public RectTransform RectTransform { get => rectTransform; }


	public void SetName(string name)
	{
		name_text.text = name;
		rectTransform.sizeDelta = new Vector2(name_text.preferredWidth + 2 * boundary, 50);
	}

	public void UpdateItem(params object[] data)
	{
		if (data != null && data.Length > 0)
		{
			try
			{
				int id = (int)data[0];
				SetName(LuaManager.Instance.GetTagInfo(id).Get<string>("name"));
			}
			catch
			{

			}
		}
	}
}
