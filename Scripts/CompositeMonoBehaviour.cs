using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviourSystem
{
    /// <summary>
    /// 更新を一箇所にまとめる
    /// </summary>
    public class CompositeMonoBehaviour : IDisposable
    {
        private readonly List<ICompositeObject> compositeObjectList = new List<ICompositeObject>();
        private bool isDisposed = false;
        private bool isRegistered = false;

        /// <summary>
        /// nullが含まれていれば自動で削除する
        /// </summary>
        /// <value></value>
        public bool IsAutoNullRemove { get; set; } = true;

        /// <summary>
        /// Registerが呼ばれたら自動で更新前にソートする
        /// </summary>
        /// <value></value>
        public bool IsAutoSort { get; set; } = true;

        public void FixedUpdate()
        {
            Foreach(compositeObject => compositeObject.OnFixedUpdate());
        }

        public void Update()
        {
            Foreach(compositeObject => compositeObject.OnUpdate());
        }

        public void LateUpdate()
        {
            Foreach(compositeObject => compositeObject.OnLateUpdate());
        }

        public void OnDestroy()
        {
            Dispose();
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
        }

        /// <summary>
        /// 昇順にソートする
        /// </summary>
        public void Sort()
        {
            compositeObjectList.Sort((a, b) => b.UpdateOrder - a.UpdateOrder);
        }

        private void Foreach(Action<ICompositeObject> action)
        {
            if (isDisposed == true)
            {
                return;
            }

            if (IsAutoSort == true && isRegistered == true)
            {
                Sort();
            }

            for (int i = compositeObjectList.Count - 1; i >= 0; i--)
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