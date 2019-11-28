using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using YuoTools;

[ExecuteInEditMode]
public class AutoLayout : MonoBehaviour
{
    RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        LayoutModle.auto = this;
        offSet.auto = this;
    }
    void Start()
    {
        //GetAllItem();
        //ReSetPos();
        SetAll();
    }
    [SerializeField]
    List<AutoLayoutItem> AllItem = new List<AutoLayoutItem>();
    List<AutoLayoutItem> AllItemTemp = new List<AutoLayoutItem>();
    public void GetAllItem()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            AutoLayoutItem item = transform.GetChild(i).GetComponent<AutoLayoutItem>();
            if (item != null)
            {
                AllItem.Add(item);
            }
        }
    }
    public void SetAll()
    {
        AllItemTemp.Clear();
        foreach (var item in AllItem)
        {
            AllItemTemp.Add(item);
        }
        foreach (var item in AllItem)
        {
            AllItemTemp[item.transform.GetSiblingIndex()] = item;
        }
    }
    public bool isUpdate;
    private void Update()
    {
        if (isUpdate) ReSet();
    }
    public AutoLayoutModle LayoutModle;
    public OffSet offSet;
    public float Space;
    int LastAllItemCount;
    private void ReSet()
    {
        Rank();
        ResetAnchor();
        ReSetPos();
    }
    void ReSetPos()
    {
        Temp.Float = 0;
        for (int i = 0; i < AllItem.Count; i++)
        {
            if (!AllItem[i].gameObject.activeSelf)
                continue;
            Temp.Float -= AllItem[i].Rect.sizeDelta.y;
            if (i > 0 && i < AllItem.Count) Temp.Float -= Space;
            Temp.V2.Set(offSet.左, Temp.Float);
            AllItem[i].Rect.anchoredPosition = Temp.V2;
        }
    }

    void ResetAnchor()
    {
        Temp.V2.Set(0.5f, 1);
        rect.pivot = Temp.V2;
        Temp.V2.Set(rect.sizeDelta.x, -Temp.Float);
        rect.sizeDelta = Temp.V2;
        Temp.Float = 0;
        for (int i = 0; i < AllItem.Count; i++)
        {
            AllItem[i].IsChirent(this);
            if (!AllItem[i].gameObject.activeSelf)
                continue;
            Temp.V2.Set((float)LayoutModle.左右 / 10, 1);
            AllItem[i].Rect.anchorMax = Temp.V2;
            AllItem[i].Rect.anchorMin = Temp.V2;
            Temp.V2.Set((float)LayoutModle.左右 / 10, 0);
            AllItem[i].Rect.pivot = Temp.V2;
            Temp.Float -= AllItem[i].Rect.sizeDelta.y;
            if (i > 0 && i < AllItem.Count) Temp.Float -= Space;
        }
    }

    public void AddItem(AutoLayoutItem item)
    {
        AllItem.Remove(item);
        AllItem.Add(item);
        if(rect) ReSet();
    }

    public void RemoveItem(AutoLayoutItem item)
    {
        AllItem.Remove(item);
        if(rect) ReSet();
    }

    void Rank()
    {
        AllItem.Sort(delegate (AutoLayoutItem x, AutoLayoutItem y)
        {
            return x.transform.GetSiblingIndex() > y.transform.GetSiblingIndex() ? 1 : -1;
        });
    }
    public enum AutoEnumLR
    {
        左 = 0,
        中 = 5,
        右 = 10
    }
    public enum AutoEnumUD
    {
        上 = 10,
        中 = 5,
        下 = 0
    }
    [Serializable]
    public class AutoLayoutModle
    {
        [SerializeField]
        private AutoEnumLR _左右;
        [SerializeField]
        private AutoEnumUD _上下;

        public AutoEnumLR 左右 { get => _左右; set { _左右 = value; auto.ReSet(); } }
        public AutoEnumUD 上下 { get => _上下; set { _上下 = value; auto.ReSet(); } }
        [HideInInspector]
        public AutoLayout auto;
    }

    [Serializable]
    public class OffSet
    {
        [SerializeField]
        private float _左;
        [SerializeField]
        private float _上;

        public float 左 { get => _左; set{ _左 = value; auto.ReSet(); } }
        public float 上 { get => _上; set { _上 = value; auto.ReSet(); } }
        [HideInInspector]
        public AutoLayout auto;
    }
}
