using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace YuoTools.Chat
{
    [System.Serializable]
    public class MessageData
    {
        public UserData user;
        public string Message;
        public Sprite image = null;
        public Vector2 Rect;
        public Vector2 TextRect;
    }

    [System.Serializable]
    public class UserData
    {
        public Sprite Head;
        public string UserName;
    }

    public class ChatManager : SingletonMono<ChatManager>
    {
        public YuoChatLoopList loopList;
        public Sprite[] Heads = new Sprite[10];
        public TextMeshProUGUI Check;
        public string Player;
        public List<UserData> us = new List<UserData>();
        public YuoChatLoopListItem CheckItem;

        private void Start()
        {
            us.Add(new UserData() { UserName = "系统提示", Head = Heads[0] });
            //创建用户
            for (int i = 0; i < 10; i++)
            {
                UserData user = new UserData();
                user.UserName = $"用户 { Random.Range(0, 99999).ToString("00000")} ";
                user.Head = Heads[i];
                us.Add(user);
            }
            //设置当前用户
            Player = us[4].UserName;

            //先生成一下随机的消息
            //InitMessage();

            //添加ui事件
            TMPText.onSubmit.AddListener(x => YuoSendMessage());
            send.onClick.AddListener(() => YuoSendMessage());
            //YuoInput.Instance.Add(
            //    new YuoInput.InputItem("Test")
            //    {
            //        key = KeyCode.A,
            //        OnHold = () =>
            //        {
            //            string str = "";
            //            int a = Random.Range(0, 100);
            //            for (int j = 0; j < a; j++)
            //            {
            //                str += $"<sprite={ Random.Range(0, 14)}>";
            //            }
            //            Send(str, us[Random.Range(0, us.Count)]);
            //        }
            //    });

        }

        public Button send;
        public TMP_InputField TMPText;

        private void InitMessage()
        {
            List<MessageData> MessageDatas = new List<MessageData>();
            List<Vector2> rects = new List<Vector2>();
            Check.gameObject.Show();
            for (int i = 0; i < 10; i++)
            {
                MessageData data = new MessageData();
                data.user = us[Random.Range(0, 9)];
                //data.Message = $"这是第{i}句话,是 {data.user.UserName}的胡言乱语";
                int a = Random.Range(0, 100);
                for (int j = 0; j < a; j++)
                {
                    data.Message += $"<sprite={ Random.Range(0, 14)}>";
                }
                Check.text = data.Message;
                Temp.V2.x = LayoutUtility.GetPreferredWidth(Check.rectTransform).RClamp(400);
                Check.rectTransform.sizeDelta = Temp.V2;
                Temp.V2.y = LayoutUtility.GetPreferredHeight(Check.rectTransform);
                data.TextRect = Temp.V2;
                if (Temp.V2.y > 60)
                {
                    Temp.V2.SetY(40 + Temp.V2.y);
                }
                else
                {
                    Temp.V2.y = 100;
                }
                data.Rect = Temp.V2;
                rects.Add(data.Rect);
                MessageDatas.Add(data);
            }
            Check.gameObject.Hide();
            loopList.SetDataCustomSize(MessageDatas, rects);
        }
        public UnityAction<string> OnSendMessage;
        public void YuoSendMessage()
        {
            OnSendMessage?.Invoke(TMPText.text);
            Send(TMPText.text, us[4]);
            TMPText.text = null;
        }
        public void Send(string message, UserData user)
        {
            Check.gameObject.Show();
            MessageData data = new MessageData();
            data.user = user;
            data.Message = message;
            Check.text = message;
            Temp.V2.x = LayoutUtility.GetPreferredWidth(Check.rectTransform).RClamp(400);
            Check.rectTransform.sizeDelta = Temp.V2;
            Temp.V2.y = LayoutUtility.GetPreferredHeight(Check.rectTransform);
            Check.rectTransform.sizeDelta = Temp.V2;
            data.TextRect = Temp.V2;
            if (data.image && CheckItem.image.rectTransform.sizeDelta.y > Temp.V2.y)
            {
                Temp.V2.y = CheckItem.image.rectTransform.sizeDelta.y;
            }
            if (Temp.V2.y > 60)
                Temp.V2.SetY(40 + Temp.V2.y);
            else
                Temp.V2.y = 100;

            data.Rect = Temp.V2;
            Check.gameObject.Hide();
            loopList.AddDataOnEnd(data, data.Rect);
            loopList.LocateRenderItemAtTarget(data, 0.3f);
        }
        public void Creat()
        {
            Check.gameObject.Show();

            List<MessageData> MessageDatas2 = new List<MessageData>();
            List<Vector2> rects2 = new List<Vector2>();
            for (int i = 0; i < 10; i++)
            {
                MessageData data = new MessageData();
                data.user = us[Random.Range(0, 9)];
                //data.Message = $"这是第{i}句话,是 {data.user.UserName}的胡言乱语";
                int a = Random.Range(0, 100);
                for (int j = 0; j < a; j++)
                {
                    data.Message += $"<sprite={ Random.Range(0, 14)}>";
                }
                Check.text = data.Message;
                Temp.V2.Set(400, 30);
                //Temp.V2.x = LayoutUtility.GetPreferredWidth(Check.rectTransform).RClamp(400);
                Check.rectTransform.sizeDelta = Temp.V2;
                Temp.V2.y = LayoutUtility.GetPreferredHeight(Check.rectTransform);
                if (Temp.V2.y > 60)
                {
                    Temp.V2.SetY(40 + Temp.V2.y);
                }
                else
                {
                    Temp.V2.y = 100;
                }
                data.Rect = Temp.V2;
                rects2.Add(data.Rect);
                MessageDatas2.Add(data);
            }
            Check.gameObject.Hide();

            loopList.AddDataCSOnStart(MessageDatas2, rects2);
        }

        public void Creat2()
        {
            Check.gameObject.Show();

            List<MessageData> MessageDatas2 = new List<MessageData>();
            List<Vector2> rects2 = new List<Vector2>();
            for (int i = 0; i < 10; i++)
            {
                MessageData data = new MessageData();
                data.user = us[Random.Range(0, 9)];
                //data.Message = $"这是第{i}句话,是 {data.user.UserName}的胡言乱语";
                int a = Random.Range(0, 100);
                for (int j = 0; j < a; j++)
                {
                    data.Message += $"<sprite={ Random.Range(0, 14)}>";
                }
                Check.text = data.Message;
                Temp.V2.Set(400, 30);
                //Temp.V2.x = LayoutUtility.GetPreferredWidth(Check.rectTransform).RClamp(400);
                Check.rectTransform.sizeDelta = Temp.V2;
                Temp.V2.y = LayoutUtility.GetPreferredHeight(Check.rectTransform);
                if (Temp.V2.y > 60)
                {
                    Temp.V2.SetY(40 + Temp.V2.y);
                }
                else
                {
                    Temp.V2.y = 100;
                }
                data.Rect = Temp.V2;
                rects2.Add(data.Rect);
                MessageDatas2.Add(data);
            }
            Check.gameObject.Hide();

            loopList.AddDataCSOnEnd(MessageDatas2, rects2);
        }
    }
}