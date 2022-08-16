using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using YuoTools;

namespace YuoTools.Chat
{
    public class YuoChatLoopListItem : YuoLoopListItem<MessageData>
    {
        public Image Head;
        public TextMeshProUGUI Message;
        public Image image;
        public TextMeshProUGUI UserName;
        private RectTransform rect;

        private void Awake()
        {
            rect = transform as RectTransform;
            Hpos = (Head.transform.parent as RectTransform).anchoredPosition.x;
            Mpos = (Message.transform.parent as RectTransform).anchoredPosition.x;
        }

        private float Mpos;
        private float Hpos;

        protected override void OnRenderer()
        {
            Message.text = mData.Message;
            UserName.text = mData.user.UserName;
            image.sprite = mData.image;
            image.gameObject.SetActive(image.sprite);
            Head.sprite = mData.user.Head;
            // Temp.V2.Set(400, 30);
            // Temp.V2.x = LayoutUtility.GetPreferredWidth(Message.rectTransform).RClamp(40, 400);
            // Message.rectTransform.sizeDelta = Temp.V2;
            // Temp.V2.y = LayoutUtility.GetPreferredHeight(Message.rectTransform).RClamp(40, 999999);
            Message.enableWordWrapping = true;
            Message.rectTransform.sizeDelta = mData.TextRect;
            rect.sizeDelta = rect.sizeDelta.RSetY(mDRect.mRect.size.y);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(Message.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(Message.transform.parent as RectTransform);

            if (mData.user.UserName == ChatManager.Instance.Player)
            {
                var r = Head.transform.parent as RectTransform;
                r.pivot = Vector2.one;
                r.anchorMax = Vector2.one;
                r.anchorMin = Vector2.one;
                r.anchoredPosition = r.anchoredPosition.RSetX(-Hpos);

                r = Message.transform.parent as RectTransform;
                r.pivot = Vector2.one;
                r.anchorMax = Vector2.one;
                r.anchorMin = Vector2.one;
                r.anchoredPosition = r.anchoredPosition.RSetX(-Mpos);
                UserName.gameObject.Hide();
            }
            else
            {
                UserName.gameObject.Show();
                var r = Head.transform.parent as RectTransform;
                r.pivot = Vector2.up;
                r.anchorMax = Vector2.up;
                r.anchorMin = Vector2.up;
                r.anchoredPosition = r.anchoredPosition.RSetX(Hpos);

                r = Message.transform.parent as RectTransform;
                r.pivot = Vector2.up;
                r.anchorMax = Vector2.up;
                r.anchorMin = Vector2.up;
                r.anchoredPosition = r.anchoredPosition.RSetX(Mpos);
            }
        }
    }
}