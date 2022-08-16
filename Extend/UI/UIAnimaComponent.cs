using DG.Tweening;
using ET;
using Unity.VisualScripting;
using UnityEngine;
using YuoTools.ECS;
using YuoTools.Extend.UI;
using YuoTools.Main.Ecs;

namespace YuoTools.UI
{
    public partial class UIAnimaComponent : YuoComponent
    {
        public RectTransform rectTransform;

        public UISetting.UISate Sate;

        /*---------------逻辑-------------*/
        private DOTweenAnimation animation;

        public async ETTask Open()
        {
            if (animation)
            {
                animation.DOPlayForward();
                Sate = UISetting.UISate.ShowAnima;
                await YuoWait.WaitTimeAsync(animation.duration);
            }

            Sate = UISetting.UISate.Show;
        }

        public async ETTask Close()
        {
            if (animation)
            {
                animation.DOPlayBackwards();
                Sate = UISetting.UISate.HideAnima;
                await YuoWait.WaitTimeAsync(animation.duration);
            }

            Sate = UISetting.UISate.Hide;
        }

        public void From(UISetting anima)
        {
            rectTransform = anima.transform as RectTransform;
            animation = anima.GetComponent<DOTweenAnimation>();
        }
    }
}