using System;
using System.Collections.Generic;
using ET;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.ECS;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class UIComponent : YuoComponent
    {
        public RectTransform rectTransform;
        public string ViewName;
        [ReadOnly] [SerializeField] int Layer;

        [LabelText("模块UI")] [ReadOnly] [Tooltip("悬浮在其他界面之上的UI")]
        public bool ModuleUI = false;

        [HideInInspector] public bool AutoShow = true;
        [HideInInspector] public bool AutoHide = true;
    }

    public partial class UIManagerComponent : YuoComponent
    {
        [ReadOnly] [SerializeField] Dictionary<string, UIComponent> UiItems = new();
        [ReadOnly] [SerializeField] Dictionary<Type, UIComponent> UiItemsType = new();
        [ReadOnly] [SerializeField] List<UIComponent> ModuleUiItems = new();

        public string DefWindow = "Test";

        [ReadOnly] [SerializeField] List<UIComponent> OpenItems = new List<UIComponent>();

        public int BaseIndex = -1;

        public string LoadPath = "Prefabs/UI/";

        [ReadOnly] public LoadType loadType = LoadType.Resources;

        Transform _transform;
        public static UIManagerComponent Instance => World.Main.GetComponent<UIManagerComponent>();

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GameObject.Find("MainCanvas").transform;
                }

                return _transform;
            }
        }

        public enum LoadType
        {
            Resources,
            AssetBundle,
            Addressable
        }

        public UILoadComponent LoadComponent;
    }

    public abstract class UILoadComponent : YuoComponent
    {
        public abstract ETTask<GameObject> Load(string path, Transform parent);
    }

    /// <summary>
    /// 处于顶部的UI
    /// </summary>
    public class TopViewComponent : YuoComponent
    {
    }

    #region 接口

    public interface IUIOpen : ISystemTag
    {
    }

    public interface IUIClose : ISystemTag
    {
    }

    public interface IUICreate : ISystemTag
    {
    }

    public interface IUIActive : ISystemTag
    {
    }

    public static partial class SystemType
    {
        public static readonly Type UIOpen = typeof(IUIOpen);
        public static readonly Type UIClose = typeof(IUIClose);
        public static readonly Type UICreate = typeof(IUICreate);
        public static readonly Type UIActive = typeof(IUIActive);
    }

    #endregion
}