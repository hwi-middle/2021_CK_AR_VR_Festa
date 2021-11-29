using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private new Renderer renderer;

    //싱글톤 처리
    private static Fader _instance;

    public static Fader Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.FindWithTag("Fader");
            if (go == null)
            {
                Debug.LogError("Fader not found");
            }

            _instance = go.GetComponent<Fader>();
        }
    }

    void Awake()
    {
        Init();
        renderer = GetComponent<Renderer>();
    }

    private IEnumerator FadeInTexture(float t)
    {
        Color color = renderer.material.color;
        while (renderer.material.color.a < 1f)
        {
            color.a += Time.deltaTime / t;
            renderer.material.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOutTexture(float t)
    {
        Color color = renderer.material.color;
        while (renderer.material.color.a > 0f)
        {
            color.a -= Time.deltaTime / t;
            renderer.material.color = color;
            yield return null;
        }
    }
}