using System;
using System.Threading.Tasks;
using ET;
using UnityEngine;
using YuoTools.ECS;
using YuoTools.Extend.UI;
using YuoTools.Main.Ecs;
using static YuoTools.Extend.UI.UISetting;
using Object = UnityEngine.Object;

namespace YuoTools.UI
{
    //这个其实属于system,但是因为扩展方法必须是public,想用就得写个友元类,所以采用分布类
    // system文件里面的组件类都属于system
    public partial class UIManagerComponent
    {
        public async Task<UIComponent> Open(string winName, GameObject go = null)
        {
            UIComponent component = Get(winName) ?? await AddWindow(winName, go);

            if (!OpenItems.Contains(component)) OpenItems.Add(component);
            else
            {
                Debug.LogWarning($"{winName} is already open");
                return component;
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

            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIOpen>(component);
            else
                World.Instance.RunSystemOfTag<IUIOpen>(component);

            if (!component.ModuleUI) TopView = component;

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                World.Instance.RunSystemOfTag<IUIOpen>(anima);
                await anima.Open();
            }

            return component;
        }

        public async Task<T> Open<T>(string winName) where T : UIComponent
        {
            return await Open(winName) as T;
        }

        public async void OpenWindow(string winName)
        {
            await Open(winName);
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
                    .AddChild(type, IDGenerater.GetID(winName)) as UIComponent;
            if (component == null) return null;

            component.Connect<UIComponent>();

            if (go == null) go = await Create(winName);

            //初始化窗口
            component.rectTransform = go.transform as RectTransform;

            //如果有动画组件就挂载动画组件
            if (go.TryGetComponent<UISetting>(out var uiSetting))
            {
                component.Entity.AddComponent<UIAnimaComponent>().From(uiSetting);
                component.ModuleUI = uiSetting.ModuleUI;
                go.SetActive(uiSetting.Active);
                Object.Destroy(uiSetting);
            }

            component.ViewName = winName;

            UiItems.Add(winName, component);
            UiItemsType.Add(type, component);

            if (BaseIndex == -1) BaseIndex = go.transform.GetSiblingIndex();

            //调用这个窗口的初始化系统
            World.Instance.RunSystemOfTagType(SystemType.UICreate, component.Entity);

            return component;
        }

        public async void Close(string winName)
        {
            UIComponent component = Get(winName);
            if (component == null)
            {
                $"找不到窗口{winName}".LogError();
                return;
            }

            if (component.TryGetComponent<UIAnimaComponent>(out var anima))
            {
                World.Instance.RunSystemOfTag<IUIClose>(anima);
                await anima.Close();
            }

            if (component.AutoHide) component.rectTransform.gameObject.Hide();
            if (component.ModuleUI && ModuleUiItems.Contains(component)) ModuleUiItems.Remove(component);
            if (component.rectTransform.gameObject.activeSelf) RunSystemAndChild<IUIClose>(component);
            if (OpenItems.Contains(component)) OpenItems.Remove(component);
            TopView = OpenItems.Count > 0 ? OpenItems[^1] : null;
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

        public UIComponent Get<T>() where T : UIComponent
        {
            return UiItemsType.ContainsKey(typeof(T)) ? UiItemsType[typeof(T)] : null;
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
            World.Instance.RunSystemOfTag<T>(component);
            World.Instance.RunSystemOfTag<T>(component.Entity.GetAllChild());
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
        protected override void Run(UIComponent component)
        {
            component.SetWindowLayer(World.Main.GetComponent<UIManagerComponent>().WindowCount);
            component.rectTransform.gameObject.Show();
        }
    }
}