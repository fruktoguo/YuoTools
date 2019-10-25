﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace YuoTools
{
    public class SaveGObjSate : Singleton<SaveGObjSate>
    {
        public Dictionary<Transform, Dictionary<Transform,SateData>> datas
            = new Dictionary<Transform, Dictionary<Transform,SateData>>();
        Color colorTemp = new Color(0, 0, 0, 0);
        public void Save(Transform tran)
        {
            Dictionary<Transform,SateData> dataTemp
                = new Dictionary<Transform, SateData >();
            Transform[] transforms = tran.GetComponentsInChildren<Transform>();
            Image[] colors = tran.GetComponentsInChildren<Image>();
            dataTemp.Clear();
            foreach (var item in transforms)
            {
                if (tran != item)
                {
                    dataTemp.Add(item,new SateData (item.gameObject.activeSelf, item.position, item.rotation, item.localScale, Color.white, null));
                }
            }
            foreach (var item in colors)
            {
                if (dataTemp.ContainsKey(item.transform))
                {
                    dataTemp[item.transform].color = item.color;
                    dataTemp[item.transform].Image = item;
                }
            }
            if (!datas.ContainsKey(tran))
            {
                datas.Add(tran, dataTemp);
            }
            else
            {
                datas[tran] = dataTemp;
            }
        }
        public void Load(Transform tran)
        {
            if (!datas.ContainsKey(tran))
                return;
            foreach (var item in datas[tran])
            {
                if (item.Key != null)
                {
                    item.Key.position = item.Value.pos;
                    item.Key.rotation = item.Value.rot;
                    item.Key.localScale = item.Value.scale;
                    item.Key.gameObject.SetActive(item.Value.active);
                    if (item.Value.Image != null)
                    {
                        item.Value.Image.color = item.Value.color;
                    }
                }
            }
        }


        public class SateData
        {
            public bool active;
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scale;
            public Color color;
            public Image Image;

            public SateData(bool active, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Image image)
            {
                this.active = active;
                this.pos = pos;
                this.rot = rot;
                this.scale = scale;
                this.color = color;
                Image = image;
            }
        }
    }
}