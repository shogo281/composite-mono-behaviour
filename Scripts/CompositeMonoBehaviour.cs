using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviour
{
    /// <summary>
    /// 更新を一箇所にまとめる
    /// </summary>
    public class CompositeMonoBehaviour : MonoBehaviour
    {
        private readonly List<ICompositeObject> compositeObjectList = new List<ICompositeObject>();

        private void FixedUpdate()
        {
            Foreach(compositeObject => compositeObject.OnFixedUpdateComposite());
        }

        private void Update()
        {
            Foreach(compositeObject => compositeObject.OnUpdateComposite());
        }

        private void LateUpdate()
        {
            Foreach(compositeObject => compositeObject.OnLateUpdateComposite());
        }

        private void OnGUI()
        {
            Foreach(compositeObject => compositeObject.OnGUIComposite());
        }

        private void OnDestroy()
        {
            compositeObjectList.Clear();
        }

        public void Register(ICompositeObject compositeObject)
        {
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

        public bool Unregister(ICompositeObject compositeObject)
        {
            if (compositeObject == null)
            {
#if UNITY_EDITOR
                Debug.LogError(new NullReferenceException());
#endif
                return false;
            }

            return compositeObjectList.Remove(compositeObject);
        }

        private void Foreach(Action<ICompositeObject> action)
        {
            for (int i = compositeObjectList.Count - 1; i > -1; i++)
            {
                action.Invoke(compositeObjectList[i]);
            }
        }
    }
}