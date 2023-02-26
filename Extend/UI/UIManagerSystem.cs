using System.Threading.Tasks;
using ET;
using UnityEngine;
using YuoTools.ECS;
using YuoTools.Extend;
using YuoTools.Extend.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class UIManagerComponent
    {
        public async Task<UIComponent> Open(string winName, GameObject go = null)
        {
            UIComponent component = Get(winName) ?? await AddWindow(winName, go);

            if (!OpenItems.Contains(component)) OpenItems.Add(component);
            // else
            // {
            //     Debug.LogWarning($"{winName} is already open");
            //     return component;
            // }

            if (component.AutoShow) component.rectTransform.gameObject.Show();

            if (component.ModuleUI)
            {
                if (!ModuleUiItems.Contains(component))
                {
                    ModuleUiItems.Add(component);
                    component.AddComponent<TopViewComponent>();
                }

                component.rectTransform.SetAsLastSibling();
            }
            else
            {
                component.SetWindowLayer(Transform.childCount - 1 - ModuleUiItems.Count);
            }

            if (!component.DefShow)
            {
                component.rectTransform.gameObject.Hide();
                component.DefShow = true;
                return component;
            }

            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIOpen>(component);
            else
                World.RunSystem<IUIOpen>(component);

            if (!component.ModuleUI) TopView = component;

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                World.RunSystem<IUIOpen>(anima);
                await anima.Open();
            }

            return component;
        }

        public async ETTask<T> Open<T>() where T : UIComponent
        {
            if (UiItemsType.ContainsKey(typeof(T)))
                return await Open(UiItemsType[typeof(T)].ViewName) as T;
            else
            {
                Debug.LogError($"UIManagerComponent Open<T> error,not find {typeof(T)}");
                return null;
            }
        }

        public async void Open<T>(T component) where T : UIComponent
        {
            if (!OpenItems.Contains(component)) OpenItems.Add(component);
            else
            {
                return;
            }

            if (component.AutoShow) component.rectTransform.gameObject.Show();

            if (component.ModuleUI)
            {
                if (!ModuleUiItems.Contains(component))
                {
                    ModuleUiItems.Add(component);
                    component.AddComponent<TopViewComponent>();
                }

                component.rectTransform.SetAsLastSibling();
            }
            else
            {
                component.SetWindowLayer(Transform.childCount - 1 - ModuleUiItems.Count);
            }

            if (!component.DefShow)
            {
                component.rectTransform.gameObject.Hide();
                component.DefShow = true;
                return;
            }

            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIOpen>(component);
            else
                World.RunSystem<IUIOpen>(component);

            if (!component.ModuleUI) TopView = component;

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                World.RunSystem<IUIOpen>(anima);
                await anima.Open();
            }
        }

        async Task<UIComponent> AddWindow(string winName, GameObject go = null)
        {
            var type = World.Instance.GetComponentType($"View_{winName}Component");
            if (type == null)
            {
                Debug.LogError($"找不到对应类型---View_{winName}Component");
                return null;
            }

            //生成窗口
            var component =
                World.Main.GetComponent<UIManagerComponent>().Entity
                    .AddChild(type, IDGenerate.GetID(winName)) as UIComponent;
            if (component == null) return null;

            component.Connect<UIComponent>();
            component.AddComponent<UIAutoExitComponent>();

            if (go == null) go = await Create(winName);

            //初始化窗口
            component.rectTransform = go.transform as RectTransform;

            //如果有动画组件就挂载动画组件
            if (go.TryGetComponent<UISetting>(out var uiSetting))
            {
                component.Entity.AddComponent<UIAnimaComponent>().From(uiSetting);
                component.ModuleUI = uiSetting.ModuleUI;
                go.SetActive(uiSetting.Active);
                component.DefShow = uiSetting.Active;
                Object.Destroy(uiSetting);
            }

            component.ViewName = winName;

            UiItems.Add(winName, component);
            UiItemsType.Add(type, component);

            if (BaseIndex == -1) BaseIndex = go.transform.GetSiblingIndex();

            //调用这个窗口的初始化系统
            component.Entity.EntityName = "View_" + component.ViewName;

            World.RunSystem<IUICreate>(component.Entity);

            return component;
        }

        public void Close<T>() where T : UIComponent
        {
            if (UiItemsType.ContainsKey(typeof(T)))
                Close(UiItemsType[typeof(T)].ViewName);
            else
            {
                Debug.LogError($"UIManagerComponent Close<T> error,not find {typeof(T)}");
            }
        }

        public async void Close(string winName)
        {
            UIComponent component = Get(winName);
            if (component == null)
            {
                $"找不到窗口{winName}".LogError();
                return;
            }

            if (component.DefShow && component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                World.RunSystem<IUIClose>(anima);
                await anima.Close();
            }

            if (component.AutoHide) component.rectTransform.gameObject.Hide();
            if (component.ModuleUI && ModuleUiItems.Contains(component)) ModuleUiItems.Remove(component);
            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIClose>(component);
            if (OpenItems.Contains(component)) OpenItems.Remove(component);
            if (!component.ModuleUI) TopView = OpenItems.Count > 0 ? OpenItems[^1] : null;
        }

        public void CloseAll()
        {
            var openItemsCopy = OpenItems.ToArray();
            foreach (var item in openItemsCopy)
            {
                Close(item.ViewName);
            }
        }

        public void Remove(string winName)
        {
            var win = Get(winName);
            if (win != null)
            {
                UiItems.Remove(winName);
                UiItemsType.Remove(win.Type);
                win.Entity.Dispose();
            }
        }

        public bool IsOpen(string winName)
        {
            return OpenItems.Contains(Get(winName));
        }

        public UIComponent Get(string winName)
        {
            return UiItems.ContainsKey(winName) ? UiItems[winName] : null;
        }

        public T Get<T>() where T : UIComponent
        {
            return UiItemsType.ContainsKey(typeof(T)) ? UiItemsType[typeof(T)] as T : null;
        }

        ETTask<GameObject> Create(string winName)
        {
            return LoadComponent.Load(LoadPath + winName, Transform);
        }

        [SerializeField] private UIComponent _topView;

        public UIComponent TopView
        {
            get => _topView;
            private set
            {
                if (_topView != value)
                {
                    if (value != null)
                    {
                        value.Entity.AddComponent<TopViewComponent>();
                        _topView?.Entity.RemoveComponent<TopViewComponent>();

                        RunSystemAndChild<IUIActive>(value);
                    }

                    _topView = value;
                }
            }
        }

        public int WindowCount => UiItems.Count + BaseIndex;

        void RunSystemAndChild<T>(UIComponent component) where T : ISystemTag
        {
            World.RunSystem<T>(component);
            World.RunSystem<T>(component.Entity.Children);
        }
    }

    public partial class UIComponent
    {
        public void SetWindowLayer(int layer)
        {
            rectTransform.SetSiblingIndex(layer);
        }
    }

    public class UIManagerSystem : YuoSystem<UIManagerComponent>, IAwake
    {
        public override string Group => "MainUI";

        protected override void Run(UIManagerComponent component)
        {
            //初始化加载系统
            switch (component.loadType)
            {
                case UIManagerComponent.LoadType.Resources:
                    component.LoadComponent = component.Entity.AddComponent<UILoadComponentResources>();
                    break;
                case UIManagerComponent.LoadType.AssetBundle:
                    break;
                case UIManagerComponent.LoadType.Addressable:
                    component.LoadComponent = component.Entity.AddComponent<UILoadComponentAddressable>();
                    break;
                default:
                    break;
            }
        }
    }

    public class UIItemOpenSystem : YuoSystem<UIComponent>, IUIOpen
    {
        public override string Group => "MainUI";

        protected override void Run(UIComponent component)
        {
            component.SetWindowLayer(World.Main.GetComponent<UIManagerComponent>().WindowCount);
            component.rectTransform.gameObject.Show();
        }
    }

    public class UIAutoExitComponent : YuoComponent
    {
        public override string Name => "场景切换时自动销毁";
    }

    /// <summary>
    /// 切换场景时清理UI
    /// </summary>
    public class UIAutoExitSystem : YuoSystem<UIAutoExitComponent>, ISceneExit
    {
        public override string Group => "MainUI";

        protected override void Run(UIAutoExitComponent component)
        {
            component.Entity.Destroy();
        }
    }
}