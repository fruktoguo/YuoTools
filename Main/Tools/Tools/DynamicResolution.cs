using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// ����֡�ʶ�̬�޸ķֱ���
/// </summary>
public class DynamicResolution : MonoBehaviour
{
    [Header("���ʱ��")]
    public float showTime = 1f;

    private int count = 0;
    private float deltaTime = 0f;

    private List<float> fpsList = new List<float>();
    private int[] resolutionWidths = new int[3];
    private int lowestFps = 50;

    private void Start()
    {
        InitSolutions();
        SetResolution(resolutionWidths[0]);
    }

    private void Update()
    {
        count++;
        deltaTime += Time.deltaTime;
        if (deltaTime >= showTime)
        {
            float fps = count / deltaTime;
            float milliSecond = deltaTime * 1000 / count;
            count = 0;
            deltaTime = 0f;

            SetResolutionReduce(fps);
        }
    }

    private void InitSolutions()
    {
        int currentWidth = Screen.currentResolution.width;
        for (int i = 0; i < resolutionWidths.Length; i++)
        {
            resolutionWidths[i] = currentWidth - (int)(currentWidth * (i + 1) * 0.1f);
        }
    }

    private void SetResolution(int width)
    {
        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;
        int height = currentHeight * width / currentWidth;
        Screen.SetResolution(width, height, true);
        Debug.Log("ǿ�ƽ��ͷֱ��� Ŀ��ֱ���Ϊ��" + width + " " + height + "  ��ǰ�ֱ��ʣ�" + currentWidth + " " + currentHeight);
    }

    private void SetResolutionReduce(float fps)
    {
        if (fps < lowestFps)
        {
            fpsList.Add(fps);
        }
        else
        {
            fpsList.Clear();
        }
        if (fpsList.Count < 5) return;
        fpsList.RemoveRange(0, 3);

        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;
        if (currentWidth < resolutionWidths[resolutionWidths.Length - 1])
        {
            return;
        }

        foreach (int width in resolutionWidths)
        {
            if (width < currentWidth)
            {
                int height = currentHeight * width / currentWidth;
                Screen.SetResolution(width, height, true);
                Debug.Log("����֡��̫�ͣ�ǿ�ƽ��ͷֱ��� Ŀ��ֱ���Ϊ��" + width + " " + height + "  ��ǰ�ֱ��ʣ�" + currentWidth + " " + currentHeight);
                break;
            }
        }
    }
}