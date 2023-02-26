using System;
using System.Collections.Generic;
using ET;
using Sirenix.OdinInspector;
using UnityEngine;
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

        [HideInInspector] public bool DefShow = true;
        [HideInInspector] public bool AutoShow = true;
        [HideInInspector] public bool AutoHide = true;
    }

    [AutoAddToMain()]
    public partial class UIManagerComponent : YuoComponentInstance<UIManagerComponent>
    {
        [ReadOnly] [SerializeField] Dictionary<string, UIComponent> UiItems = new();
        [ReadOnly] [SerializeField] Dictionary<Type, UIComponent> UiItemsType = new();
        [ReadOnly] [SerializeField] List<UIComponent> ModuleUiItems = new();

        public string DefWindow = "Test";

        public override string Name => "UI管理器";

        [ReadOnly] [SerializeField] List<UIComponent> OpenItems = new List<UIComponent>();

        public int BaseIndex = -1;

        public string LoadPath = "Prefabs/UI/";

        [ReadOnly] public LoadType loadType = LoadType.Resources;

        Transform _transform;

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GameObject.Find("Canvas").transform;
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
        public override string Name => "当前UI处于顶部";
    }

    #region 接口

    /// <summary>
    ///  当UI打开时调用一次
    /// </summary>
    public interface IUIOpen : ISystemTag
    {
    }

    /// <summary>
    ///  当UI关闭时调用一次
    /// </summary>
    public interface IUIClose : ISystemTag
    {
    }

    /// <summary>
    /// 在UI创建时调用一次,在Awake之后
    /// </summary>
    public interface IUICreate : ISystemTag
    {
    }

    /// <summary>
    ///  当UI处于顶层时调用一次
    /// </summary>
    public interface IUIActive : ISystemTag
    {
    }

    /// <summary>
    ///  UI手动切换自适应时调用
    /// </summary>
    public interface IUIAdaption : ISystemTag
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