using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviourSystem
{
    /// <summary>
    /// 更新を一箇所にまとめる
    /// </summary>
    public class CompositeMonoBehaviour : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// OnGUIが呼ばれたときのイベント
        /// </summary>
        public event Action OnGUICalled = null;
        private readonly List<ICompositeObject> compositeObjectList = new List<ICompositeObject>();
        private bool isDisposed = false;

        /// <summary>
        /// nullが含まれていれば自動で削除するか
        /// </summary>
        /// <value></value>
        public bool IsAutoNullRemove { get; set; } = true;

        private void FixedUpdate()
        {
            Foreach(compositeObject => compositeObject.OnFixedUpdate());
        }

        private void Update()
        {
            Foreach(compositeObject => compositeObject.OnUpdate());
        }

        private void LateUpdate()
        {
            Foreach(compositeObject => compositeObject.OnLateUpdate());
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void OnGUI()
        {
            if (OnGUICalled != null)
            {
                OnGUICalled.Invoke();
            }
        }

        /// <summary>
        /// 登録する
        /// </summary>
        /// <param name="compositeObject"></param>
        public void Register(ICompositeObject compositeObject)
        {
            if (isDisposed == true)
            {
                return;
            }

            if (compositeObject == null)
            {
#if UNITY_EDITOR
                Debug.LogError(new NullReferenceException());
#endif
                return;
            }

            if (compositeObjectList.Contains(compositeObject) == true)
            {
#if UNITY_EDITOR
                Debug.LogError("すでに追加されているICompositeObjectです。");
#endif
                return;
            }

            compositeObjectList.Add(compositeObject);
        }

        /// <summary>
        /// 登録を解除する
        /// </summary>
        /// <param name="compositeObject"></param>
        /// <returns></returns>
        public bool Unregister(ICompositeObject compositeObject)
        {

            if (isDisposed == true)
            {
                return false;
            }

            if (compositeObject == null)
            {
#if UNITY_EDITOR
                Debug.LogError(new NullReferenceException());
#endif
                return false;
            }

            return compositeObjectList.Remove(compositeObject);
        }

        /// <summary>
        /// 破棄する
        /// </summary>
        public void Dispose()
        {
            if (isDisposed == true)
            {
                return;
            }

            isDisposed = true;
            compositeObjectList.Clear();
            OnGUICalled = null;
        }

        private void Foreach(Action<ICompositeObject> action)
        {
            if (isDisposed == false)
            {
                return;
            }

            for (int i = compositeObjectList.Count - 1; i > -1; i++)
            {
                var obj = compositeObjectList[i];

                if (obj == null)
                {
                    if (IsAutoNullRemove == true)
                    {
                        compositeObjectList.RemoveAt(i);
                        continue;
                    }

#if UNITY_EDITOR
                    Debug.LogError("nullが含まれています。");
#endif

                }
                action.Invoke(obj);
            }
        }
    }
}