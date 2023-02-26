#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using YuoTools.Main.Ecs;

namespace YuoTools.Extend
{
    [AutoAddToMain]
    public class ComponentManager : YuoComponent
    {
        private ComponentManagerAsset _asset;

        //通过Resources加载保存配置文件
        public ComponentManagerAsset Asset
        {
            get
            {
                //如果没有加载过
                if (_asset == null)
                {
                    //加载配置文件
                    _asset = Resources.Load<ComponentManagerAsset>("ComponentManagerAsset");
                    //如果没有配置文件
                    if (_asset == null)
                    {
                        //创建配置文件
                        _asset = ScriptableObject.CreateInstance<ComponentManagerAsset>();
#if UNITY_EDITOR
                        //保存配置文件
                        AssetDatabase.CreateAsset(_asset, "Assets/Resources/ComponentManagerAsset.asset");
#endif
                    }
                }

                return _asset;
            }
        }
#if UNITY_EDITOR
        public void Save()
        {
            //保存配置文件
            EditorUtility.SetDirty(Asset);
        }
#endif
    }
#if UNITY_EDITOR

    public class ComponentManagerSystem : YuoSystem<ComponentManager>, IAwake, IExitGame
    {
        protected override void Run(ComponentManager component)
        {
            if (RunType == SystemType.ExitGame)
            {
                component.Save();
            }
        }
    }
#endif
}