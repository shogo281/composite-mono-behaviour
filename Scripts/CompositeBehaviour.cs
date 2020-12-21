using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CB
{
    public class CompositeBehaviour : IDisposable
    {
        private class UnregisterInfo
        {
            public int order = -1;
            public IBehaviour compositedObject = null;
        }

        private readonly List<UnregisterInfo> unregisterCompositeList = null;
        private readonly List<IBehaviour> registerCompositeList = null;
        private readonly List<IBehaviour>[] compositeArray = null;
        private bool isDisposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="capacity"></param>
        public CompositeBehaviour(int capacity = 1)
        {
            registerCompositeList = new List<IBehaviour>();
            unregisterCompositeList = new List<UnregisterInfo>();
            compositeArray = new List<IBehaviour>[capacity];

            for (int i = 0; i < capacity; i++)
            {
                compositeArray[i] = new List<IBehaviour>();
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

            for (int i = 0; i < compositeArray.Length; i++)
            {
                var list = compositeArray[i];

                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
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

            for (int i = 0; i < compositeArray.Length; i++)
            {
                var list = compositeArray[i];

                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
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

            for (int i = 0; i < compositeArray.Length; i++)
            {
                var list = compositeArray[i];

                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    item.OnLateUpdate();
                }
            }
        }

        /// <summary>
        /// 登録する
        /// </summary>
        /// <param name="compositeObject"></param>
        public void Register(IBehaviour compositeObject)
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

            if (compositeArray[compositeObject.UpdateOrder].Contains(compositeObject) == true)
            {
#if UNITY_EDITOR
                Debug.LogError("すでに追加されているICompositeObjectです。");
#endif
                return;
            }

            registerCompositeList.Add(compositeObject);

            var index = unregisterCompositeList.FindIndex(info => info.compositedObject == compositeObject);

            if (index != -1)
            {
                unregisterCompositeList.RemoveAt(index);
            }
        }

        /// <summary>
        /// 登録を解除する
        /// </summary>
        /// <param name="compositeObject"></param>
        /// <returns></returns>
        public bool Unregister(IBehaviour compositeObject)
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

            var canRemove = compositeArray[compositeObject.UpdateOrder].Contains(compositeObject);

            registerCompositeList.Remove(compositeObject);

            if (unregisterCompositeList.Exists(info => info.compositedObject == compositeObject) == false && canRemove == true)
            {
                unregisterCompositeList.Add(new UnregisterInfo()
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

            for (var i = 0; i < compositeArray.Length; i++)
            {
                compositeArray[i].Clear();
            }
        }

        /// <summary>
        /// 登録する
        /// </summary>
        private void Register()
        {
            for (var i = 0; i < registerCompositeList.Count; i++)
            {
                var behaviour = registerCompositeList[i];
                var order = behaviour.UpdateOrder;

                if (compositeArray[order].Contains(behaviour) == true)
                {
                    continue;
                }
                compositeArray[order].Add(behaviour);
                behaviour.OnRegister();
            }

            registerCompositeList.Clear();
        }

        /// <summary>
        /// 登録を解除する
        /// </summary>
        private void Unregister()
        {
            for (var i = 0; i < unregisterCompositeList.Count; i++)
            {
                var unregisterInfo = unregisterCompositeList[i];

                if (compositeArray[unregisterInfo.order].Remove(unregisterInfo.compositedObject) == true)
                {
                    unregisterInfo.compositedObject.OnUnregister();
                }
            }

            unregisterCompositeList.Clear();
        }
    }
}