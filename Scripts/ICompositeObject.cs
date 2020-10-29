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
    public interface ICompositedObject
    {
        /// <summary>
        /// 更新の優先順位
        /// </summary>
        /// <value></value>
        int UpdateOrder { get; }

        void OnFixedUpdate();
        void OnUpdate();
        void OnLateUpdate();
    }
}