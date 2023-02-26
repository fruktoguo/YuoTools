using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

namespace YuoTools
{
    public static class YuoJsonHelper
    {
        private static string defPath;

        public static string DefPath
        {
            get => defPath; set
            {
                if (value != null && value != "")
                {
                    defPath = value + "/";
                }
                else
                {
                    defPath = null;
                }
            }
        }

        public static T Load<T>(string path)
        {
            string localPath = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                localPath = Application.streamingAssetsPath + "/" + path;
            }
            else
            {
                localPath = "file:///" + Application.streamingAssetsPath + "/" + path;
            }
            UnityWebRequest requrest = UnityWebRequest.Get(localPath);
            var operation = requrest.SendWebRequest();
            while (!operation.isDone)
            { }
            return JsonUtility.FromJson<T>(requrest.downloadHandler.text);
        }

        public static void Save<T>(string path, T data)
        {
            string localPath = null;
            if (Application.platform == RuntimePlatform.Android)
            {
                localPath = $"file:///{Application.streamingAssetsPath}/{DefPath}{path}";
            }
            else
            {
                localPath = $"{Application.streamingAssetsPath}/{DefPath}{path}";
            }
            //??????¡¤??
            FileInfo file = new FileInfo(localPath);
            //?§Ø?????????????????????????§Õ?????????
            StreamWriter sw = file.CreateText();
            sw.WriteLine(JsonUtility.ToJson(data));
            sw.Close();
            sw.Dispose();
        }
    }
}