using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace YuoTools
{
    public class SaveGObjSate : Singleton<SaveGObjSate>
    {
        public Dictionary<Transform, Dictionary<Transform, (bool active, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Image Image)>> datas
            = new Dictionary<Transform, Dictionary<Transform, (bool active, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Image Image)>>();
        Dictionary<Transform, (bool active, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Image Image)> dataTemp
            = new Dictionary<Transform, (bool active, Vector3 pos, Quaternion rot, Vector3 scale, Color color, Image Image)>();
        Color colorTemp = new Color(0, 0, 0, 0);
        public void Save(Transform tran)
        {
            Transform[] transforms = tran.GetComponentsInChildren<Transform>();
            Image[] colors = tran.GetComponentsInChildren<Image>();
            dataTemp.Clear();
            foreach (var item in transforms)
            {
                if (tran != item)
                {
                    dataTemp.Add(item, (item.gameObject.activeSelf, item.position, item.rotation, item.localScale, Color.white, null));
                }
            }
            foreach (var item in colors)
            {
                if (dataTemp.ContainsKey(item.transform))
                {
                    dataTemp[item.transform] = (dataTemp[item.transform].active, dataTemp[item.transform].pos, dataTemp[item.transform].rot, dataTemp[item.transform].scale, item.color, item);
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
            dataTemp = datas[tran];
            foreach (var item in dataTemp)
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
    }
}