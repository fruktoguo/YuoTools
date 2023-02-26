using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using System.IO;
using UnityEngine.AddressableAssets;

namespace YuoTools
{
    public class AssetsLoader : SingletonMono<AssetsLoader>
    {
        //需要导入Addressable包,如果不想使用改成false
#if true
        private Dictionary<string, PoolItem> AllPools = new Dictionary<string, PoolItem>();

        private Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        public static void InstantiateAsync(string name, UnityAction<GameObject> Completed, int MaxPoolCount = 0)
        {
            if (!Instance.AllPools.ContainsKey(name))
            {
                Instance.AllPools.Add(name, new PoolItem(name, MaxPoolCount));
                //$"创建了个新的池子<{name}>".Log();
            }

            if (Instance.AllPools[name].Pools.Count > 0)
            {
                Instance.AllPools[name].Pools[0].ResetTrans();
                Instance.AllPools[name].Pools[0].ReShow();
                Completed?.Invoke(Instance.AllPools[name].Pools[0]);
                Instance.AllPools[name].Pools.RemoveAt(0);
            }
            else
            {
                Addressables.InstantiateAsync($"{name}.prefab").Completed += x =>
                {
                    x.Result.transform.SetParent(Instance.transform);
                    Completed?.Invoke(x.Result);
                    //移除的时候根据name分配池子
                    x.Result.name = name;
                };
            }
        }

        public static void InstantiateEffectsAsync(string name, UnityAction<GameObject> Completed, int MaxPoolCount = 0)
        {
            InstantiateAsync($"Effects/{name}", Completed, MaxPoolCount);
        }

        public static void GetSprite(string name, UnityAction<Sprite> Completed, bool Save = true)
        {
            if (Instance.Sprites.ContainsKey(name))
            {
                Completed?.Invoke(Instance.Sprites[name]);
            }
            else
            {
                Addressables.LoadAssetAsync<Sprite>(name).Completed += x =>
                {
                    //Get.Sprites.Add(name, Sprite.Create(x.Result, new Rect(0, 0, x.Result.width, x.Result.height), new Vector2(0.5f, 0.5f)));

                    if (!Instance.Sprites.ContainsKey(name))
                    {
                        Instance.Sprites.Add(name, x.Result);
                    }

                    Completed?.Invoke(Instance.Sprites[name]);
                    if (!Save)
                    {
                        Instance.Sprites.Remove(name);
                    }
                };
            }
        }

        public static void GetSpriteatlas(string SpriteAtlasName, string SpriteName, UnityAction<Sprite> Completed)
        {
            if (Instance.Sprites.ContainsKey($"{SpriteAtlasName}_{SpriteName}"))
            {
                Completed?.Invoke(Instance.Sprites[$"{SpriteAtlasName}_{SpriteName}"]);
            }
            else
            {
                Addressables.LoadAssetAsync<SpriteAtlas>(SpriteAtlasName).Completed += x =>
                {
                    Sprite[] spriteArray = new Sprite[x.Result.spriteCount];
                    //spriteArray得到数组
                    x.Result.GetSprites(spriteArray);
                    foreach (var item in spriteArray)
                    {
                        item.name = item.name.Replace("(Clone)", string.Empty);
                        item.name = $"{SpriteAtlasName}_{item.name}";
                        if (!Instance.Sprites.ContainsKey(item.name))
                        {
                            Instance.Sprites.Add(item.name, item);
                        }
                    }

                    Completed?.Invoke(Instance.Sprites[$"{SpriteAtlasName}_{SpriteName}"]);
                };
            }
        }

        public static void ReMove(GameObject game)
        {
            if (Instance.AllPools.ContainsKey(game.name))
            {
                if (Instance.AllPools[game.name].MaxPoolCount > Instance.AllPools[game.name].Pools.Count)
                {
                    Instance.AllPools[game.name].Pools.Add(game);
                    game.Hide();
                }
                else
                {
                    Destroy(game);
                }
            }
            else
            {
                Destroy(game);
            }
        }

#endif
        public Dictionary<string, float> LoadProgress = new();

        public void StartProgress(string s, float progress)
        {
            LoadProgress.TryAdd(s, progress);
        }
        
        public void EndProgress(string s)
        {
            LoadProgress.Remove(s);
        }

        public static byte[] Load(string path)
        {
            //创建文件读取流
            FileStream _fileStream = new FileStream(path, FileMode.Open);
            _fileStream.Seek(0, SeekOrigin.Begin);
            //创建文件长度缓冲区
            byte[] _bytes = new byte[_fileStream.Length];
            _fileStream.Read(_bytes, 0, (int)_fileStream.Length);
            _fileStream.Close();
            _fileStream.Dispose();
            return _bytes;
        }

        public static string LoadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public class PoolItem
        {
            public PoolItem(string name, int max)
            {
                PreName = name;
                MaxPoolCount = max;
            }

            public string PreName;
            public int MaxPoolCount = 10;
            public List<GameObject> Pools = new List<GameObject>();
        }
    }
}