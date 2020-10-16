using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviourSystem
{
    /// <summary>
    /// CompositeMonoBehaviourに追加できるオブジェクト
    /// ライフサイクルは通常のMonoBehaviourと同じ
    /// </summary>
    public interface ICompositeObject
    {
        void OnFixedUpdate();
        void OnUpdate();
        void OnLateUpdate();
    }
}