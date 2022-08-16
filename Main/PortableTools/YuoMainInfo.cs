using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YuoMainInfo
{
    public static class ScreenInfo
    {
        //程序刷新率
        static float targetScreenRefreshRate = 0;

        public static float TargetRefreshRate
        {
            get
            {
                if (targetScreenRefreshRate == 0)
                {
                    targetScreenRefreshRate = Screen.currentResolution.refreshRate;
                    if (targetScreenRefreshRate > 1000)
                    {
                        targetScreenRefreshRate /= 1000;
                    }
                }

                return targetScreenRefreshRate;
            }
            set
            {
                targetScreenRefreshRate = value;
                Application.targetFrameRate = (int) targetScreenRefreshRate;
            }
        }

        //屏幕刷新率
        static float screenRefreshRate = 0;

        public static float RefreshRate
        {
            get
            {
                if (screenRefreshRate == 0)
                {
                    screenRefreshRate = Screen.currentResolution.refreshRate;
                    if (screenRefreshRate > 1000)
                    {
                        screenRefreshRate /= 1000;
                    }
                }

                return screenRefreshRate;
            }
        }

        //屏幕大小
        static float screenSize = 0;

        public static float ScreenSize
        {
            get
            {
                if (screenSize == 0)
                {
                    screenSize = new Vector2(Screen.width, Screen.height).magnitude;
                }

                return screenSize;
            }
        }
    }
}