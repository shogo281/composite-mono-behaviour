using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviourSystem
{
    public class CompositeMonoBehaviour : IDisposable
    {
        private class UnregisterInfo
        {
            public int order = -1;
            public ICompositedObject compositedObject = null;
        }

        private readonly List<UnregisterInfo> unregisterCompositedList = null;
        private readonly List<ICompositedObject> registerCompositedList = null;
        private readonly List<ICompositedObject>[] compositedArray = null;
        private bool isDisposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="capacity"></param>
        public CompositeMonoBehaviour(int capacity = 1)
        {
            registerCompositedList = new List<ICompositedObject>();
            unregisterCompositedList = new List<UnregisterInfo>();
            compositedArray = new List<ICompositedObject>[capacity];

            for (int i = 0; i < capacity; i++)
            {
                compositedArray[i] = new List<ICompositedObject>();
            }
        }

        public void FixedUpdate()
        {
            if (isDisposed == true)
            {
                return;
            }

            Register();
            Unregister();

            for (int i = 0; i < compositedArray.Length; i++)
            {
                var list = compositedArray[i];

                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];

                    if (j == null)
                    {
                        continue;
                    }

                    item.OnFixedUpdate();
                }
            }
        }

        public void Update()
        {
            if (isDisposed == true)
            {
                return;
            }

            Register();
            Unregister();

            for (int i = 0; i < compositedArray.Length; i++)
            {
                var list = compositedArray[i];

                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];

                    if (j == null)
                    {
                        continue;
                    }

                    item.OnUpdate();
                }
            }
        }

        public void LateUpdate()
        {
            if (isDisposed == true)
            {
                return;
            }
            Register();
            Unregister();

            for (int i = 0; i < compositedArray.Length; i++)
            {
                var list = compositedArray[i];

                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];

                    if (j == null)
                    {
                        continue;
                    }

                    item.OnLateUpdate();
                }
            }
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

            if (compositedArray[compositeObject.UpdateOrder].Contains(compositeObject) == true)
            {
#if UNITY_EDITOR
                Debug.LogError("すでに追加されているICompositeObjectです。");
#endif
                return;
            }

            registerCompositedList.Add(compositeObject);

            var index = unregisterCompositedList.FindIndex(info => info.compositedObject == compositeObject);

            if (index != -1)
            {
                unregisterCompositedList.RemoveAt(index);
            }
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

            var canRemove = compositedArray[compositeObject.UpdateOrder].Contains(compositeObject);

            registerCompositedList.Remove(compositeObject);

            if (unregisterCompositedList.Exists(info => info.compositedObject == compositeObject) == false && canRemove == true)
            {
                unregisterCompositedList.Add(new UnregisterInfo()
                {
                    order = compositeObject.UpdateOrder,
                    compositedObject = compositeObject
                });
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

            for (int i = 0; i < compositedArray.Length; i++)
            {
                compositedArray[i].Clear();
            }
        }

        private void Register()
        {
            for (int i = 0; i < registerCompositedList.Count; i++)
            {
                var obj = registerCompositedList[i];
                var order = obj.UpdateOrder;
                if (compositedArray[order].Contains(obj) == false)
                {
                    compositedArray[order].Add(obj);
                }
            }

            registerCompositedList.Clear();
        }

        private void Unregister()
        {
            for (int i = 0; i < unregisterCompositedList.Count; i++)
            {
                var unregisterObj = unregisterCompositedList[i];
                compositedArray[unregisterObj.order].Remove(unregisterObj.compositedObject);
            }

            unregisterCompositedList.Clear();
        }
    }
}