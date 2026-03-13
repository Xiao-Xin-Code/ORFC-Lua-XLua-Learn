using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TagUnit : MonoBehaviour,
    IPointerDownHandler,IPointerUpHandler
{
    [SerializeField]
    TMP_Text name_text;
    [SerializeField]
    Image image;

    public bool isOn;

    public int ID { get; private set; }


    public void SetIsOn(bool state)
    {
        string colorState = state ? "select_color" : "default_color";
        string infoColorState = state ? "select_info_color" : "default_info_color";
        isOn = state;
        image.color = LuaManager.Instance.GetTagColor(ID, colorState);
        name_text.color = LuaManager.Instance.GetTagColor(ID, infoColorState);
	}


	public void OnPointerDown(PointerEventData eventData)
	{
        if (!isOn)
        {
            if(LuaManager.Instance.tagCombines.Count >= 5)
            {
                Debug.LogWarning("选取Tag超过5个！当前仅作警告，不限制选取个数");
            }
        }

        SetIsOn(!isOn);
		//isOn = !isOn;
        if (isOn)
        {
            if (!LuaManager.Instance.tagCombines.Contains(ID))
            {
                LuaManager.Instance.tagCombines.Add(ID);
			}
        }
        else
        {
			if (LuaManager.Instance.tagCombines.Contains(ID))
			{
				LuaManager.Instance.tagCombines.Remove(ID);
			}
		}
        LuaManager.Instance.Search_Roles();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		
	}

	public void SetID(int id)
    {
        ID = id;
    }


    public void SetName(string name)
    {
        name_text.text = name;
    }
}
