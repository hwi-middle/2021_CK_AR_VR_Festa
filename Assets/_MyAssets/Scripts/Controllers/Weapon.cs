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
        Debug.Log($"ChangeColor");

        Color color = Color.white;

        switch(colorType)
        {
            case EColor.Red:
                color = Color.red;
                break;
            case EColor.Green:
                color = Color.green;
                break;
            case EColor.Blue:
                color = Color.blue;
                break;
            default:
                Debug.Assert(false);
                break;
        }

        myRenderer.material.color = color;

    }
}