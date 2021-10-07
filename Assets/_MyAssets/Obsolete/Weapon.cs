using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private EColor currentColor = EColor.Default;
    private Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = transform.GetChild(0).GetComponent<Renderer>();
        myRenderer.material.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shot()
    {

    }

    public void ChangeColor(EColor colorType)
    {
        Color resColor = Color.white;

        switch(colorType)
        {
            case EColor.Red:
                if(currentColor == EColor.Yellow || currentColor == EColor.Blue)
                {
                    EColor res = ColorUtility.MixColor(currentColor, colorType);
                    currentColor = res;
                    resColor = ColorUtility.EnumColorToUnityColor(res);
                }
                else
                {
                    currentColor = EColor.Red;
                    resColor = ColorUtility.red;
                }
                break;
            case EColor.Yellow:
                if (currentColor == EColor.Red || currentColor == EColor.Blue)
                {
                    EColor res = ColorUtility.MixColor(currentColor, colorType);
                    currentColor = res;
                    resColor = ColorUtility.EnumColorToUnityColor(res);
                }
                else
                {
                    currentColor = EColor.Yellow;
                    resColor = ColorUtility.yellow;
                }
                break;
            case EColor.Blue:
                if (currentColor == EColor.Red || currentColor == EColor.Yellow)
                {
                    EColor res = ColorUtility.MixColor(currentColor, colorType);
                    currentColor = res;
                    resColor = ColorUtility.EnumColorToUnityColor(res);
                }
                else
                {
                    currentColor = EColor.Blue;
                    resColor = ColorUtility.blue;
                }
                break;
            case EColor.Default:
                resColor = Color.white;
                currentColor = EColor.Default;
                break;
            default:
                Debug.Assert(false);
                break;
        }
        myRenderer.material.color = resColor;
    }
}