using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField] private OVRScreenFade ovrScreenFade;
    [SerializeField] private Text engindDialog;
    private AudioSource _audioSource;
    [SerializeField] private Image buttonHider;
    [SerializeField] private GameObject buttons;

    private string dialog = "...그날 나는 사장을 땅에 묻었다.\n"
                            + "...물론 내 마음 속에서.\n\n"
                            + "사장을 때리는 상상을 했지만 현실에서는 그럴 수 없었다.\n"
                            + "나는 사장이 준 폐기 상품들을 들고 집으로 돌아왔다.\n\n"
                            + "편의점 알바는 쉬운 일이 아니구나.\n"
                            + "나는 저런 손님들이 되지 말아야겠다.\n"
                            + "그리고 아르바이트 할 때는 계약서를 꼭 쓰자...";

    // Start is called before the first frame update
    void Start()
    {
        if (OVRPlugin.userPresent)
        {
            InputManager.Recenter();
        }

        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(Proceed());
    }

    void Update()
    {
        if (InputManager.GetDown(InputManager.Button.Thumbstick, InputManager.Controller.RTouch))
        {
            InputManager.Recenter();
        }
    }

    private IEnumerator Proceed()
    {
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);

        float delay = 0.1f;
        yield return new WaitForSeconds(delay);

        char lastCh = Char.MaxValue;
        foreach (var t in dialog)
        {
            if (t == '\n')
            {
                if (lastCh != '\n')
                {
                    yield return new WaitForSeconds(1.0f);
                }

                engindDialog.text += t;
                lastCh = t;
                continue;
            }

            engindDialog.text += t;
            _audioSource.Play();
            yield return new WaitForSeconds(delay);
            lastCh = t;
        }

        buttons.SetActive(true);
        Color color = buttonHider.color;
        float time2 = 2.0f;
        while (buttonHider.color.a > 0f)
        {
            color.a -= Time.deltaTime / time2;
            buttonHider.color = color;
            yield return null;
        }
    }

    public void GoToCreditSceneBtn()
    {
        StartCoroutine(GoToCreditScene());
    }

    private IEnumerator GoToCreditScene()
    {
        ovrScreenFade.FadeOut();
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);
        SceneManager.LoadScene("Credit");
    }
}