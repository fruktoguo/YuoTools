using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using YuoTools.Main.Ecs;
using YuoTools.UI;

namespace YuoTools.Extend.UI
{
    public class UISetting : MonoBehaviour
    {
        [HorizontalGroup] [InfoBox("如果不需要动态创建就勾上\n注意需要默认显示才行")] [LabelText("是否不需要动态创建")]
        public bool DontIstantiate = false;

        [HorizontalGroup] [Header("默认显示状态")] public bool Active = true;

        [HorizontalGroup("2")] [LabelText("模块UI")]
        public bool ModuleUI = false;

#if UNITY_EDITOR

        [HorizontalGroup("2")]
        [Button("生成UI代码")]
        public void SwapCode()
        {
            SpawnUICode.SpawnCode(gameObject);
        }
#endif

        private async void Start()
        {
            if (DontIstantiate)
            {
                await World.Main.GetComponent<UIManagerComponent>().Open(gameObject.name, gameObject);
            }
        }

        private Animator animator;

        public Animator Animator
        {
            get
            {
                if (!animator) animator = GetComponent<Animator>();
                return animator;
            }
        }

        // ReSharper disable once NotAccessedField.Local
        [SerializeField] private bool openTools = false;

        [ShowIf("openTools", true)] [FoldoutGroup("Raycast")]
        public List<MaskableGraphic> maskableGraphics = new List<MaskableGraphic>();

        [ShowIf("openTools", true)]
        [FoldoutGroup("Raycast")]
        [Button(ButtonHeight = 30, Name = "获取所有开启了Raycast的物体")]
        public void FindAllRaycast()
        {
            maskableGraphics.Clear();
            foreach (var item in transform.GetComponentsInChildren<MaskableGraphic>())
            {
                if (item.raycastTarget)
                {
                    maskableGraphics.Add(item);
                }
            }
        }

        [ShowIf("openTools", true)]
        [FoldoutGroup("Raycast")]
        [Button(ButtonHeight = 30, Name = "清除剩余Raycast")]
        public void CloseRaycast()
        {
            foreach (var item in maskableGraphics)
            {
                item.raycastTarget = false;
            }
        }

        public enum UISate
        {
            Hide = 0,
            Show = 1,
            ShowAnima = 2,
            HideAnima = 3,
        }
    }
}