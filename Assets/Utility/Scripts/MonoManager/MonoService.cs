namespace Frame
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// Mono业务逻辑管理单例
    /// </summary>
    public sealed partial class MonoService : AutoSingleton<MonoService>
    {
        public MonoService() { }

        #region 协程
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            if (routine != null)
            {
                return MonoRuntime.Instance.StartCoroutine(routine);
            }
            else
            {
                return null;
            }

        }

        public void StopCoroutine(IEnumerator routine)
        {
            if (routine != null)
            {
                MonoRuntime.Instance.StopCoroutine(routine);
            }
        }

        public void StopCoroutine(Coroutine coroutine)
        {
			if (coroutine != null)
			{
				MonoRuntime.Instance.StopCoroutine(coroutine);
			}
		}

        public void StopAllCoroutines()
        {
            MonoRuntime.Instance.StopAllCoroutines();
        }
        #endregion

        #region Update
        public void AddUpdateListener(UnityAction call)
        {
            MonoRuntime.Instance.AddUpdateListener(call);
        }

        public void RemoveUpdateListener(UnityAction call)
        {
            MonoRuntime.Instance.RemoveUpdateListener(call);
        }

        public void RemoveAllUpdateListeners()
        {
            MonoRuntime.Instance.RemoveAllUpdateListeners();
        }
        #endregion

        #region FixedUpdate
        public void AddFixedUpdateListener(UnityAction call)
        {
            MonoRuntime.Instance.AddFixedUpdateListener(call);
        }

        public void RemoveFixedUpdateListener(UnityAction call)
        {
            MonoRuntime.Instance.RemoveFixedUpdateListener(call);
        }

        public void RemoveAllFixedUpdateListeners()
        {
            MonoRuntime.Instance.RemoveAllFixedUpdateListeners();
        }
        #endregion

        #region LateUpdate
        public void AddLateUpdateListener(UnityAction call)
        {
            MonoRuntime.Instance.AddLateUpdateListener(call);
        }

        public void RemoveLateUpdateListener(UnityAction call)
        {
            MonoRuntime.Instance.RemoveLateUpdateListener(call);
        }

        public void RemoveAllLateUpdateListeners()
        {
            MonoRuntime.Instance.RemoveAllLateUpdateListeners();
        }
        #endregion

    }
}