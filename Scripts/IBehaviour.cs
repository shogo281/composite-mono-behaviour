using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CB
{
    /// <summary>
    /// CompositeMonoBehaviourに追加する用のインターフェース
    /// </summary>
    public interface IBehaviour
    {
        /// <summary>
        /// 更新の優先順位
        /// </summary>
        /// <value></value>
        int UpdateOrder { get; }

        void OnFixedUpdate();
        void OnUpdate();
        void OnLateUpdate();
        void OnRegister();
        void OnUnregister();
    }
}