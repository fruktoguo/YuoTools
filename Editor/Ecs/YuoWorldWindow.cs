using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.Editor.Ecs
{
    public class YuoWorldWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/World &w")]
        private static async void OpenWindow()
        {
            var window = GetWindow<YuoWorldWindow>();
            window.Show();
            await Task.Delay(100);
            window.ForceMenuTreeRebuild();
        }

        float _time;
        string _searchEntity = "";

        string SearchEntity
        {
            get => _searchEntity;
            set
            {
                if (value != _searchEntity)
                {
                    _searchEntity = value;
                    ForceMenuTreeRebuild();
                }
            }
        }

        string _searchSystem = "";

        string SearchSystem
        {
            get => _searchSystem;
            set
            {
                if (value != _searchSystem)
                {
                    _searchSystem = value;
                    ForceMenuTreeRebuild();
                }
            }
        }

        protected override void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("请先运行游戏");
                return;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Toggle(_isEntities, "Entities", EditorStyles.toolbarButton) != _isEntities)
            {
                _isEntities = !_isEntities;
                _isSystems = !_isEntities;
                ForceMenuTreeRebuild();
            }

            if (GUILayout.Toggle(_isSystems, "Systems", EditorStyles.toolbarButton) != _isSystems)
            {
                _isSystems = !_isSystems;
                _isEntities = !_isSystems;
                ForceMenuTreeRebuild();
            }

            if (GUILayout.Button("刷新", GUILayout.Width(50)))
            {
                ForceMenuTreeRebuild();
            }

            GUILayout.EndHorizontal();
            if (_isEntities)
            {
                SearchEntity = GUILayout.TextField(SearchEntity, (GUIStyle)"SearchTextField");
            }
            else
            {
                SearchSystem = GUILayout.TextField(SearchSystem, (GUIStyle)"SearchTextField");
            }

            foreach (var systemView in _systemViews)
            {
                systemView.Update();
            }

            foreach (var entityView in _componentViews)
            {
                entityView.Update();
            }

            base.OnGUI();
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            if (_isSystems) return;
            if (_lastCount != World.Instance.Entities.Count)
            {
                ForceMenuTreeRebuild();
                _lastCount = World.Instance.Entities.Count;
            }
        }

        int _lastCount = 0;

        bool _isEntities = true;
        bool _isSystems = false;


        readonly List<ComponentView> _componentViews = new List<ComponentView>();
        readonly List<SystemView> _systemViews = new List<SystemView>();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                DefaultMenuStyle =
                {
                    AlignTriangleLeft = false,
                }
            };
            if (!Application.isPlaying)
            {
                return tree;
            }

            _componentViews.Clear();
            _systemViews.Clear();

            #region Entites

            if (_isEntities)
            {
                AddEntity(World.Main, World.Main.EntityName);
                foreach (var scene in World.Instance.AllScenes)
                {
                    AddEntity(scene, scene.EntityName);
                }
            }

            #endregion

            #region Systems

            if (_isSystems)
            {
                foreach (var system in World.Instance.GetAllSystem)
                {
                    AddSystem(system);
                }
            }

            #endregion

            return tree;

            void AddSystem(SystemBase system)
            {
                if (!string.IsNullOrEmpty(SearchSystem) &&
                    //检索名称,不区分大小写
                    !system.GetType().Name.ToLower().Contains(SearchSystem.ToLower()))
                {
                    return;
                }

                SystemView view = new SystemView();
                view.SystemOrder = system.GetType().GetCustomAttribute<SystemOrderAttribute>()?.Order ?? 0;
                view.TimeConsuming = system.TimeConsuming;
                view.System = system;
                view.InfluenceType.AddRange(system.InfluenceTypes().Select(type => type.Name));
                view.InterfaceType.AddRange(system.Type.GetInterfaces().Select(type => type.Name));
                view.InterfaceType.Remove("ISystemTag");
                tree.Add((system.Group == "" ? "" : $"{system.Group}/") + system.Name, view);
                _systemViews.Add(view);
            }

            void AddEntity(YuoEntity entity, string path)
            {
                if (string.IsNullOrEmpty(SearchEntity) ||
                    //检索名称,不区分大小写
                    entity.EntityName.ToLower().Contains(SearchEntity.ToLower()) ||
                    //检索ID
                    entity.ID.ToString().Contains(SearchEntity))
                {
                    ComponentView view = new()
                    {
                        Entity = entity
                    };
                    view.EntityID = entity.ID;
                    foreach (var component in entity.Components.Values)
                    {
                        if (!view.Components.Contains(component))
                        {
                            view.Components.Add(component);
                        }
                    }

                    _componentViews.Add(view);

                    if (view.Components.Count == 0) tree.Add("无组件", "无组件");

                    tree.Add(path, view);
                    foreach (var child in entity.Children)
                    {
                        AddEntity(child, path + "/" + child.EntityName);
                    }
                }
                else
                {
                    foreach (var child in entity.Children)
                    {
                        AddEntity(child, child.EntityName);
                    }
                }
            }
        }

        public class ComponentView
        {
            [ReadOnly] public long EntityID;

            [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false,
                ShowItemCount = false, ListElementLabelName = "@Name",
                ElementColor = "ElementColor")]
            [ShowInInspector]
            [HideIf("_count", 0)]
            public List<YuoComponent> Components = new();

            [SerializeField] [ReadOnly] public YuoEntity Entity;

            private Color ElementColor(int index)
            {
                return HashCodeToColor(Components[index].Type.GetHashCode());
            }

            private Color HashCodeToColor(int hashCode)
            {
                float h = Math.Abs(hashCode / 1.3f % 1f);
                return Color.HSVToRGB(h, 0.6f, 0.55f);
            }

            int _count;

            public void Update()
            {
                _count = Components.Count;
                if (Entity == null) return;
                Components.Clear();
                foreach (var component in Entity.Components.Values)
                {
                    if (!Components.Contains(component))
                    {
                        Components.Add(component);
                    }
                }
            }
        }

        public class SystemView
        {
            [HorizontalGroup()]
            [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false,
                ShowItemCount = false)]
            [ShowInInspector]
            [ReadOnly]
            [LabelText("支持的类型")]
            public List<string> InfluenceType = new();

            [HorizontalGroup()]
            [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false,
                ShowItemCount = false)]
            [ShowInInspector]
            [ReadOnly]
            [LabelText("触发的类型")]
            public List<string> InterfaceType = new();

            [ReadOnly] public short SystemOrder;

            [LabelText("执行耗时")] [SuffixLabel("毫秒")] [ReadOnly]
            public double TimeConsuming;

            public SystemBase System;

            public List<YuoEntity> Entities = new();

            [ListDrawerSettings(Expanded = true, HideAddButton = true, HideRemoveButton = true, DraggableItems = false,
                ShowItemCount = false)]
            [ShowInInspector]
            [ReadOnly]
            public List<string> Entity = new();

            public void Update()
            {
                TimeConsuming = System.TimeConsuming;
                Entities = System.Entitys;
                Entity.Clear();
                Entity.AddRange(Entities.Select(e => e.EntityName));
            }
        }
    }
}