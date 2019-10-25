using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YuoTools;
public class MouseClickManger : SingletonMono<MouseClickManger>
{
    public int MaxComboNum = 6;
    float LastClickTime = 0;
    float ComboTime = 0.2f;
    UnityAction[] action;
    public void SetComboTime(float time)
    {
        ComboTime = time;
    }
    public int ComboNum { get; set; } = 1;
    YuoDelayMod mod;
    public override void Awake()
    {
        base.Awake();
        action = new UnityAction[MaxComboNum];
    }
    private void Update()
    {
        Combo();
    }
    public void Combo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - LastClickTime < ComboTime)
            {
                ComboNum++;
                if (mod==null)
                {
                    mod = this.YuoDelay(() => InvokeCombo(ComboNum),ComboTime);
                }
                else
                {
                    mod.SetDelay(ComboTime);
                }
            }
            LastClickTime = Time.time;
        }
    }
    public void AddComboClick(int num,UnityAction action)
    {
        if (num<2||num>MaxComboNum)
        {
            return;
        }
        this.action[num - 2] += action;
    }
    void InvokeCombo(int num)
    {
        mod = null;
        num.Clamp(0, MaxComboNum);
        if (num <= MaxComboNum && num >= 2)
        {
            $"连击 * {num}".Log();
            action[num-2]?.Invoke();
        }
        ComboNum = 1;
    }
}
