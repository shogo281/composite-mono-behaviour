using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviourSystem
{
    public class CompositeMonoBehaviour : IDisposable
    {
        private readonly List<ICompositedObject> unregisterCompositedList = null;
        private readonly List<ICompositedObject> registerCompositedList = null;
        private readonly Dictionary<int, List<ICompositedObject>> compositedDictionary = null;
        private bool isDisposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="capacity"></param>
        public CompositeMonoBehaviour(int capacity = 1)
        {
            registerCompositedList = new List<ICompositedObject>();
            unregisterCompositedList = new List<ICompositedObject>();
            compositedDictionary = new Dictionary<int, List<ICompositedObject>>();

            for (int i = 0; i < capacity; i++)
            {
                compositedDictionary.Add(i, new List<ICompositedObject>());
            }
        }

        public void FixedUpdate()
        {
            if (isDisposed == true)
            {
                return;
            }

            Register();

            foreach (var pair in compositedDictionary)
            {
                foreach (var value in pair.Value)
                {
                    value.OnFixedUpdate();
                }
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

            foreach (var pair in compositedDictionary)
            {
                foreach (var value in pair.Value)
                {
                    value.OnUpdate();
                }
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

            foreach (var pair in compositedDictionary)
            {
                foreach (var value in pair.Value)
                {
                    value.OnLateUpdate();
                }
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

            if (compositedDictionary[compositeObject.UpdateOrder].Contains(compositeObject) == true)
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

            var canRemove = compositedDictionary[compositeObject.UpdateOrder].Contains(compositeObject);

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
            compositedDictionary.Clear();
        }

        private void Register()
        {
            foreach (var obj in registerCompositedList)
            {
                var order = obj.UpdateOrder;
                if (compositedDictionary[order].Contains(obj) == false)
                {
                    compositedDictionary[order].Add(obj);
                }
            }

            registerCompositedList.Clear();
        }

        private void Unregister()
        {
            foreach (var unregisterObj in unregisterCompositedList)
            {
                compositedDictionary[unregisterObj.UpdateOrder].Remove(unregisterObj);
            }

            unregisterCompositedList.Clear();
        }
    }
}