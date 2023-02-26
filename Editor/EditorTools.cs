using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace YuoTools.YuoEditor
{
    [Obsolete("Obsolete")]
    public class EditorTools : UnityEditor.Editor
    {
        //获取所有粒子特效
        [MenuItem("GameObject/测试工具/获取所有粒子特效材质", false, -3)]
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

        [MenuItem("GameObject/测试工具/测试 &u", false, -3)]
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

        [MenuItem("GameObject/YuoUI_通用工具/精灵切换UI", false, -3)]
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

        [MenuItem("GameObject/YuoUI_通用工具/切换粒子效果缩放模式", false, -3)]
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

        [MenuItem("GameObject/YuoUI_通用工具/获取文件路径", false, -3)]
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
                list.Add(transform.GetChild(i));
                if (transform.GetChild(i).childCount > 0)
                {
                    list.AddRange(FindAll(transform.GetChild(i)));
                }
            }

            return list;
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换粒子透明度/五分之一", false, -3)]
        [Obsolete("Obsolete")]
        public static void SwitchParticleAlpha_L()
        {
            SwitchParticleAlpha(0.2f);
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换粒子透明度/翻倍", false, -3)]
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
                        ps.startColor = ps.startColor.RSetA(ps.startColor.a * alpha);
                    }

                    if (sr)
                    {
                        sr.color = sr.color.RSetA(sr.color.a * alpha);
                    }
                }
            }
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换选中/图片为精灵", false, -3)]
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

        [MenuItem("GameObject/YuoUI_通用工具/切换选中/特效为UI_呼吸", false, -3)]
        [Obsolete("Obsolete")]
        private static void ParticleToImage()
        {
            if (_lastTime.ApEqual(Time.realtimeSinceStartup))
            {
                return;
            }

            _lastTime = Time.realtimeSinceStartup;
            GameObject[] selectObjs = Selection.gameObjects;
            int index = 0;
            foreach (var item in selectObjs)
            {
                index++;
                var ps = item.GetComponent<ParticleSystem>();
                if (!ps) continue;
                var startColor = ps.startColor;
                var lifeColor = ps.colorOverLifetime.color;
                var go = new GameObject($"Light_{index}");
                go.transform.SetParent(ps.transform.parent);
                go.ResetTrans();
                go.AddComponent<Image>().sprite = ps.textureSheetAnimation.GetSprite(0);
                go.GetComponent<Image>().SetNativeSize();
                go.GetComponent<Image>().color = startColor;
                var icot = go.AddComponent<ImageColorOfTime>();
                icot.gradient = lifeColor.gradient;
                icot.animaTime = ps.main.startLifetime.constant;
                icot.Cycle = ps.time;
                // icot.Cycle = ps.main.s;
            }
        }

        private static float _lastTime = int.MinValue;

        [MenuItem("GameObject/YuoUI_通用工具/切换选中/特效为UI_旋转", false, -3)]
        private static void ParticleToImageRotate()
        {
            if (_lastTime.ApEqual(Time.realtimeSinceStartup))
            {
                return;
            }

            _lastTime = Time.realtimeSinceStartup;
            var selectObjs = EditorTools.GetAllSelectComponent<ParticleSystem>(true);
            int index = 0;
            foreach (var ps in selectObjs)
            {
                index++;
                if (!ps) continue;
                var startColor = ps.startColor;
                var go = new GameObject($"Rotate_{index}");
                go.transform.SetParent(ps.transform.parent);
                go.ResetTrans();
                go.AddComponent<Image>().sprite = ps.textureSheetAnimation.GetSprite(0);
                go.GetComponent<Image>().SetNativeSize();
                go.GetComponent<Image>().color = startColor;
                var icot = go.AddComponent<Anima_Rotate>();
                icot.transform.localRotation = Quaternion.Euler(0, 0, ps.main.startRotation.constant * 180 / Mathf.PI);
                //弧长
                icot.speed = ps.rotationOverLifetime.zMultiplier * 180 / Mathf.PI;
            }
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换精灵层级/-10", false, -3)]
        public static void SetSortingOrder_10()
        {
            SetSortingOrder(-10);
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换精灵层级/-1", false, -3)]
        public static void SetSortingOrder_1()
        {
            SetSortingOrder(-1);
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换精灵层级/+1", false, -3)]
        public static void SetSortingOrder1()
        {
            SetSortingOrder(1);
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换精灵层级/+10", false, -3)]
        public static void SetSortingOrder10()
        {
            SetSortingOrder(10);
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换精灵层级/+100", false, -3)]
        public static void SetSortingOrder100()
        {
            SetSortingOrder(100);
        }

        [MenuItem("GameObject/YuoUI_通用工具/切换精灵层级/-100", false, -3)]
        public static void SetSortingOrder_100()
        {
            SetSortingOrder(-100);
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

        [MenuItem("GameObject/YuoUI_通用工具/切换物体显隐状态 &q", false, -3)]
        public static void SetObjActive()
        {
            GameObject[] selectObjs = Selection.gameObjects;
            Undo.SetCurrentGroupName($"切换物体显隐状态 [数量:{selectObjs.Length}]");
            Undo.RecordObjects(selectObjs, "切换物体显隐状态");
            int objCtn = selectObjs.Length;
            for (int i = 0; i < objCtn; i++)
            {
                bool isAcitve = selectObjs[i].activeSelf;
                selectObjs[i].SetActive(!isAcitve);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("Assets/导出精灵", false, -3)]
        public static void ExportSprite()
        {
            string resourcesPath = "Assets/Resources/";
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj);
                if (selectionPath.StartsWith(resourcesPath))
                {
                    string selectionExt = System.IO.Path.GetExtension(selectionPath);
                    if (selectionExt.Length == 0)
                    {
                        Debug.LogError($"资源{selectionPath}的扩展名不对，请选择图片资源");
                        continue;
                    }

                    // 如果selectionPath = "Assets/Resources/UI/Common.png"
                    // 那么loadPath = "UI/Common"
                    string loadPath = selectionPath.Remove(selectionPath.Length - selectionExt.Length);
                    loadPath = loadPath.Substring(resourcesPath.Length);
                    // 加载此文件下的所有资源
                    Sprite[] sprites = Resources.LoadAll<Sprite>(loadPath);
                    if (sprites.Length > 0)
                    {
                        // 创建导出目录
                        string exportPath = Application.dataPath + "/ExportSprite/" + loadPath;
                        System.IO.Directory.CreateDirectory(exportPath);

                        foreach (Sprite sprite in sprites)
                        {
                            Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height,
                                sprite.texture.format, false);
                            tex.SetPixels(sprite.texture.GetPixels((int)sprite.rect.xMin, (int)sprite.rect.yMin,
                                (int)sprite.rect.width, (int)sprite.rect.height));
                            tex.Apply();

                            // 将图片数据写入文件
                            System.IO.File.WriteAllBytes(exportPath + "/" + sprite.name + ".png", tex.EncodeToPNG());
                        }

                        Debug.Log("导出精灵到" + exportPath);
                    }

                    Debug.Log("导出精灵完成");
                    // 刷新资源
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"请将资源放在{resourcesPath}目录下");
                }
            }
        }

        [MenuItem("Assets/名字全部大写", false, -3)]
        public static void RenameToUpper()
        {
            foreach (Object obj in Selection.objects)
            {
                string selectionPath = AssetDatabase.GetAssetPath(obj);
                string selectionName = System.IO.Path.GetFileNameWithoutExtension(selectionPath);
                string newName = selectionName.ToUpper();
                AssetDatabase.RenameAsset(selectionPath, newName);
                AssetDatabase.Refresh();
            }
        }
    }
}