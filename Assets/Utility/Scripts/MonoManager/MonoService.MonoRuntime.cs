namespace Frame
{
    using UnityEngine.Events;

    public sealed partial class MonoService
    {
        /// <summary>
        /// Ã·π©MonoBehaviour
        /// </summary>
        private sealed class MonoRuntime : AutoMonoSingleton<MonoRuntime>
        {
            private MonoRuntime() { }

            
            #region Event

            event UnityAction updateEvent;
            event UnityAction fixedUpdateEvent;
            event UnityAction lateUpdateEvent;

            #endregion

            private void Update() => updateEvent?.Invoke();
            private void FixedUpdate() => fixedUpdateEvent?.Invoke();
            private void LateUpdate() => lateUpdateEvent?.Invoke();



            #region Update
            internal void AddUpdateListener(UnityAction call)
            {
                updateEvent += call;
            }

            internal void RemoveUpdateListener(UnityAction call)
            {
                updateEvent -= call;
            }

            internal void RemoveAllUpdateListeners()
            {
                updateEvent = null;
            }
            #endregion

            #region FixedUpdate
            internal void AddFixedUpdateListener(UnityAction call)
            {
                fixedUpdateEvent += call;
            }

            internal void RemoveFixedUpdateListener(UnityAction call)
            {
                fixedUpdateEvent -= call;
            }

            internal void RemoveAllFixedUpdateListeners()
            {
                fixedUpdateEvent = null;
            }
            #endregion

            #region LateUpdate
            public void AddLateUpdateListener(UnityAction call)
            {
                lateUpdateEvent += call;
            }

            internal void RemoveLateUpdateListener(UnityAction call)
            {
                lateUpdateEvent -= call;
            }

            internal void RemoveAllLateUpdateListeners()
            {
                lateUpdateEvent = null;
            }
            #endregion
        }
    }

}
