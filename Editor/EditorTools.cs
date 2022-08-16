using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace YuoTools.YuoEditor
{
    public class EditorTools : UnityEditor.Editor
    {
        //获取所有粒子特效
        [MenuItem("Tools/测试工具/获取所有粒子特效材质")]
        public static void GetAllParticleSystems()
        {
            List<ParticleSystem> list = new List<ParticleSystem>();
            foreach (ParticleSystem ps in Object.FindObjectsOfType<ParticleSystem>())
            {
                list.Add(ps);
            }

            foreach (var item in list)
            {
                var render = item.GetComponent<Renderer>();
                //Debug.Log(render.sharedMaterial.name + "----" + render.sharedMaterial.shader.name);
                if (render.sharedMaterial.shader.name == "Legacy Shaders/Particles/Additive" &&
                    render.sharedMaterial.name != "Particles_Additive")
                {
                    Debug.Log(render.sharedMaterial.name);
                }

                if (render.sharedMaterial.shader.name == "Sprites-Defaulte" &&
                    render.sharedMaterial.name != "Sprites/Default")
                {
                    Debug.Log(render.sharedMaterial.name);
                }
            }
        }

        [MenuItem("Tools/测试工具/测试 &u")]
        public static void GetTest()
        {
            foreach (var item in Selection.gameObjects)
            {
                foreach (var item_1 in item.GetComponentsInChildren<Transform>())
                {
                    Debug.Log(item_1);
                }
            }
        }

        [MenuItem("Tools/通用工具/精灵切换UI")]
        public static void SwitchToUI()
        {
            GameObject[] selectObjs = Selection.gameObjects;
            foreach (var item in selectObjs)
            {
                var sr = item.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    var s = sr.sprite;
                    item.AddComponent<Image>().sprite = s;
                    DestroyImmediate(sr);
                }
            }
        }

        [MenuItem("Tools/通用工具/切换粒子效果缩放模式")]
        [System.Obsolete]
        public static void SwitchParticleScale()
        {
            foreach (var item in GetAllSelectComponent<ParticleSystem>())
            {
                Debug.Log(item.main.scalingMode);
                if (item.main.scalingMode == ParticleSystemScalingMode.Local)
                {
                    item.scalingMode = ParticleSystemScalingMode.Hierarchy;
                }
            }
        }

        [MenuItem("Tools/通用工具/获取文件路径")]
        public static void GetFilePath()
        {
            foreach (var item in Selection.assetGUIDs)
            {
                Debug.Log(AssetDatabase.GUIDToAssetPath(item));
            }
        }

        public static List<T> GetAllSelectComponent<T>(bool andThis = false) where T : Component
        {
            List<T> list = new List<T>();

            foreach (var item in Selection.transforms)
            {
                if (andThis)
                {
                    T t = item.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }

                foreach (var item_1 in FindAll(item))
                {
                    T t = item_1.GetComponent<T>();
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }

        private static List<Transform> FindAll(Transform transform)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).childCount > 0)
                {
                    list.AddRange(FindAll(transform.GetChild(i)));
                }
                else
                {
                    list.Add(transform.GetChild(i));
                }
            }

            return list;
        }

        [MenuItem("Tools/通用工具/切换粒子透明度/五分之一")]
        [Obsolete("Obsolete")]
        public static void SwitchParticleAlpha_L()
        {
            SwitchParticleAlpha(0.2f);
        }

        [MenuItem("Tools/通用工具/切换粒子透明度/翻倍")]
        [Obsolete("Obsolete")]
        public static void SwitchParticleAlpha_B()
        {
            SwitchParticleAlpha(2f);
        }

        [Obsolete("Obsolete")]
        public static void SwitchParticleAlpha(float alpha)
        {
            GameObject[] selectObjs = Selection.gameObjects;
            foreach (var item in selectObjs)
            {
                foreach (var item_1 in item.GetComponentsInChildren<Transform>())
                {
                    var ps = item_1.GetComponent<ParticleSystem>();
                    var sr = item_1.GetComponent<SpriteRenderer>();
                    if (ps)
                    {
                        ps.startColor = ps.startColor.SetA(ps.startColor.a * alpha);
                    }

                    if (sr)
                    {
                        sr.color = sr.color.SetA(sr.color.a * alpha);
                    }
                }
            }
        }

        [MenuItem("Tools/通用工具/切换选中/图片为精灵")]
        private static void EditTexture()
        {
            Object[] selects;
            selects = Selection.objects;
            foreach (var item in selects)
            {
                if (item && item is Texture)
                {
                    string path = AssetDatabase.GetAssetPath(item);
                    TextureImporter texture = AssetImporter.GetAtPath(path) as TextureImporter;
                    texture.textureType = TextureImporterType.Sprite;
                    texture.alphaIsTransparency = true;
                    texture.spritePixelsPerUnit = 1;
                    texture.spriteImportMode = SpriteImportMode.Single;
                    texture.filterMode = FilterMode.Trilinear;
                    texture.mipmapEnabled = false;
                }
            }
        }

        [MenuItem("Tools/通用工具/切换精灵层级/-10")]
        public static void SetSortingOrder_10()
        {
            SetSortingOrder(-10);
        }

        [MenuItem("Tools/通用工具/切换精灵层级/-1")]
        public static void SetSortingOrder_1()
        {
            SetSortingOrder(-1);
        }

        [MenuItem("Tools/通用工具/切换精灵层级/+1")]
        public static void SetSortingOrder1()
        {
            SetSortingOrder(1);
        }

        [MenuItem("Tools/通用工具/切换精灵层级/+10")]
        public static void SetSortingOrder10()
        {
            SetSortingOrder(10);
        }

        private static void SetSortingOrder(int i)
        {
            foreach (var item in GetAllSelectComponent<SpriteRenderer>(true))
            {
                item.sortingOrder += i;
            }

            foreach (var item in GetAllSelectComponent<ParticleSystem>(true))
            {
                item.GetComponent<Renderer>().sortingOrder += i;
            }
        }

        [MenuItem("Tools/通用工具/切换物体显隐状态 &q")]
        public static void SetObjActive()
        {
            GameObject[] selectObjs = Selection.gameObjects;
            int objCtn = selectObjs.Length;
            for (int i = 0; i < objCtn; i++)
            {
                bool isAcitve = selectObjs[i].activeSelf;
                selectObjs[i].SetActive(!isAcitve);
            }
        }


    }
}