using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YuoTools
{
    [System.Serializable]
    public class YuoAction<T> : UnityEvent<T>
    {

    }
    [System.Serializable]
    public class YuoAction : UnityEvent
    {

    }
}
