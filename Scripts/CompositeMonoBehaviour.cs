using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviourSystem
{
    public class CompositeMonoBehaviour : IDisposable
    {
        private readonly List<ICompositedObject> compositedObjectList = new List<ICompositedObject>();
        private readonly List<ICompositedObject> unregisterCompositedList = new List<ICompositedObject>();
        private readonly List<ICompositedObject> registerCompositedList = new List<ICompositedObject>();
        private bool isDisposed = false;
        private bool isRequestSort = false;

        /// <summary>
        /// Registerが呼ばれたら自動で更新前にソートする
        /// </summary>
        /// <value></value>
        public bool IsAutoSort { get; set; } = true;

        public void FixedUpdate()
        {
            if (isDisposed == true)
            {
                return;
            }

            Register();
            UpdateSort();

            foreach (var obj in compositedObjectList)
            {
                if (unregisterCompositedList.Contains(obj) == true)
                {
                    continue;
                }

                obj.OnFixedUpdate();
            }

            Unregister();
        }

        public void Update()
        {
            if (isDisposed == true)
            {
                return;
            }

            Register();
            UpdateSort();

            foreach (var obj in compositedObjectList)
            {
                if (unregisterCompositedList.Contains(obj) == true)
                {
                    continue;
                }

                obj.OnUpdate();
            }

            Unregister();
        }

        public void LateUpdate()
        {
            if (isDisposed == true)
            {
                return;
            }

            Register();
            UpdateSort();

            foreach (var obj in compositedObjectList)
            {
                if (unregisterCompositedList.Contains(obj) == true)
                {
                    continue;
                }

                obj.OnLateUpdate();
            }

            Unregister();
        }

        public void OnDestroy()
        {
            Dispose();
        }

        /// <summary>
        /// 登録する
        /// </summary>
        /// <param name="compositeObject"></param>
        public void Register(ICompositedObject compositeObject)
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

            if (compositedObjectList.Contains(compositeObject) == true)
            {
#if UNITY_EDITOR
                Debug.LogError("すでに追加されているICompositeObjectです。");
#endif
                return;
            }

            registerCompositedList.Add(compositeObject);
            unregisterCompositedList.Remove(compositeObject);
        }

        /// <summary>
        /// 登録を解除する
        /// </summary>
        /// <param name="compositeObject"></param>
        /// <returns></returns>
        public bool Unregister(ICompositedObject compositeObject)
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

            var canRemove = compositedObjectList.Contains(compositeObject);

            registerCompositedList.Remove(compositeObject);

            if (unregisterCompositedList.Contains(compositeObject) == false && canRemove == true)
            {
                unregisterCompositedList.Add(compositeObject);
            }

            return canRemove == true;
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
            compositedObjectList.Clear();
        }

        /// <summary>
        /// 昇順にソートする
        /// </summary>
        public void RequestSort()
        {
            isRequestSort = true;
        }

        private void Sort()
        {
            compositedObjectList.Sort((a, b) => a.UpdateOrder - b.UpdateOrder);
        }

        private void UpdateSort()
        {
            if (IsAutoSort == true)
            {
                Sort();
                return;
            }

            if (isRequestSort == true)
            {
                Sort();
                isRequestSort = false;
            }
        }

        private void Register()
        {
            foreach (var obj in registerCompositedList)
            {
                if (compositedObjectList.Contains(obj) == false)
                {
                    compositedObjectList.Add(obj);
                }
            }

            registerCompositedList.Clear();
        }

        private void Unregister()
        {
            foreach (var unregisterObj in unregisterCompositedList)
            {
                compositedObjectList.Remove(unregisterObj);
            }

            unregisterCompositedList.Clear();
        }
    }
}