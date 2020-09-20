using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CompositeMonoBehaviour
{
    /// <summary>
    /// CompositeMonoBehaviourに追加できるオブジェクト
    /// ライフサイクルは通常のMonoBehaviourと同じ
    /// </summary>
    public interface ICompositeObject
    {
        [Obsolete]
        void OnAwakeComposite();
        [Obsolete]
        void OnEnableCompsite();
        [Obsolete]
        void OnStartComposite();
        void OnFixedUpdateComposite();
        void OnUpdateComposite();
        void OnLateUpdateComposite();
        void OnGUIComposite();
        [Obsolete]
        void OnDisableComposite();
        [Obsolete]
        void OnDestroyComposite();
    }
}