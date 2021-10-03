using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ColorUtility
{
    static public Color none = new Color(1, 1, 1);

    static public Color red = new Color(1, 0, 0);
    static public Color yellow = new Color(1, 1, 0);
    static public Color blue = new Color(0, 0, 1);

    static public Color orange = new Color(1, 0.6f, 0);
    static public Color green = new Color(0, 1, 0);
    static public Color purple = new Color(0.6f, 0, 1);

    static public EColor MixColor(EColor a, EColor b)
    {
        //�������� int������ ĳ�����Ͽ� b�� �� ũ���� ���� ����
        if ((int)a > (int)b)
        {
            EColor temp = a;
            a = b;
            b = temp;
        }

        //�ϳ��� Default �����̸� �ٸ� �ϳ��� ������ ��ȯ
        if (a == EColor.Default) return b;
        if (b == EColor.Default) return a;

        //RYB�� ���� �� ����
        if (a == EColor.Red && b == EColor.Yellow)
        {
            return EColor.Orange;
        }
        else if (a == EColor.Red && b == EColor.Blue)
        {
            return EColor.Purple;
        }
        else if (a == EColor.Yellow && b == EColor.Blue)
        {
            return EColor.Green;
        }
        else
        {
            return EColor.Default;
        }
    }

    static public Color EnumColorToUnityColor(EColor c)
    {
        switch (c)
        {
            case EColor.Default:
                return none;
            case EColor.Red:
                return red;
            case EColor.Yellow:
                return yellow;
            case EColor.Blue:
                return blue;
            case EColor.Orange:
                return orange;
            case EColor.Green:
                return green;
            case EColor.Purple:
                return purple;
            default:
                Debug.Assert(false);
                return none;
        }
    }
}
