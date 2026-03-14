using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class InfiniteScrollView : MonoBehaviour
{
	public enum ScrollMode
	{
		Horizontal,
		Vertical,
		All
	}

	[SerializeField] ScrollRect scroll;
	[SerializeField] RectTransform rectTransform;

	List<object> datas;
	[SerializeField] ScrollMode mode;


	public void SetDatas(IEnumerable values)
	{
		datas = new List<object>();
		foreach (var item in values)
		{
			datas.Add(item);
		}
		UpdateFunction();
	}


	private void Start()
	{
		scroll.onValueChanged.AddListener(e => HorizontalFunction());
	}

	//需要获取生成项，需要有更新生成项方法
	event Func<IItem> Get;
	event UnityAction BeforeUpdate;
	event UnityAction EndUpdate;


	public void RegisterGet(Func<IItem> func)
	{
		Get = func;
	}

	public void RegisterBeforeUpdate(UnityAction action)
	{
		BeforeUpdate = action;
	}

	public void RegisterEndUpdate(UnityAction action)
	{
		EndUpdate = action;
	}



	private void UpdateFunction()
	{
		switch (mode)
		{
			case ScrollMode.Horizontal:
				HorizontalFunction();
				break;
			case ScrollMode.Vertical:
				VerticalFunction();
				break;
			case ScrollMode.All:
				AllFunction();
				break;
			default:
				break;
		}
	}


	private void HorizontalFunction()
	{
		BeforeUpdate?.Invoke();
		float totalH = rectTransform.rect.height;
		float totalW = 0;
		for(int i = 0; i < datas.Count; i++)
		{
			if(i == 0)
			{
				totalW += 10;
			}
			IItem item = Get.Invoke();
			item.RectTransform.SetParent(scroll.content);
			item.UpdateItem(datas[i]);
			float itemWidth = item.RectTransform.rect.width;
			float tempX = totalW + itemWidth / 2;
			item.RectTransform.anchoredPosition = new Vector2(tempX, 0);
			totalW += itemWidth;
			if (i < datas.Count - 1)
			{
				totalW += 10;
			}
			if(i == datas.Count - 1)
			{
				totalW += 10;
			}
		}
		scroll.content.sizeDelta = new Vector2(totalW, totalH);
		EndUpdate?.Invoke();
	}

	private void VerticalFunction()
	{
		BeforeUpdate?.Invoke();
		float totalH = 0;
		float totalW = rectTransform.rect.width;
		for (int i = 0; i < datas.Count; i++)
		{
			if (i == 0)
			{
				totalH += 10;
			}
			IItem item = Get.Invoke();
			item.RectTransform.SetParent(scroll.content);
			item.UpdateItem(datas[i]);
			float itemHeight = item.RectTransform.rect.height;
			float tempY = totalH + itemHeight / 2;
			item.RectTransform.anchoredPosition = new Vector2(0, tempY);
			totalH += itemHeight;
			if (i < datas.Count - 1)
			{
				totalH += 10;
			}
			if (i == datas.Count - 1)
			{
				totalH += 10;
			}
		}
		scroll.content.sizeDelta = new Vector2(totalW, totalH);
		EndUpdate?.Invoke();

	}


	private void AllFunction()
	{
		BeforeUpdate?.Invoke();

		//整体宽度 rectTransform.rect.width;
		//整体高度 rectTransform.rect.height;

		float totalH = 0;
		float totalW = 0;

		float curMaxHeight = 0;

		float curX = 0;

		//当前的位置
		//是否需要添加左侧间隔
		//是否添加右侧间隔
		//死否添加中间间隔

		bool isLeft = true;


		for (int i = 0; i < datas.Count; i++)
		{

			if (isLeft)
			{
				float tempx = curX + 10;

				if (tempx >= rectTransform.rect.width)
				{
					//无法在范围内显示 
					return;
				}
				else
				{
					//可以进行添加一个时的判断
				}
				
				curX += 10;

				


			}


			if (i == 0)
			{
				totalH += 10;
			}
			IItem item = Get.Invoke();
			item.RectTransform.SetParent(scroll.content);
			item.UpdateItem(datas[i]);
			float itemHeight = item.RectTransform.rect.height;
			float tempY = totalH + itemHeight / 2;
			item.RectTransform.anchoredPosition = new Vector2(0, tempY);
			totalH += itemHeight;
			if (i < datas.Count - 1)
			{
				totalH += 10;
			}
			if (i == datas.Count - 1)
			{
				totalH += 10;
			}
		}
		scroll.content.sizeDelta = new Vector2(totalW, totalH);
		EndUpdate?.Invoke();
	}
}

public interface IItem
{
	RectTransform RectTransform { get; }
	/// <summary>
	/// 只更新数据相关
	/// </summary>
	/// <param name="data"></param>
	void UpdateItem(params object[] data);
}