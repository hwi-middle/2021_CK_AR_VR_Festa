using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditSceneManager : MonoBehaviour
{
    [SerializeField] private OVRScreenFade ovrScreenFade;
    private AudioSource _audioSource;

    [SerializeField] private Image gameLogo;
    [SerializeField] private Text role;
    [SerializeField] private Text creditName;
    [SerializeField] private Image engineLogo;
    [SerializeField] private Image schoolLogo;
    [SerializeField] private Text description;
    [SerializeField] private Image teamLogo;


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (OVRPlugin.userPresent)
        {
            InputManager.Recenter();
        }

        StartCoroutine(Proceed());
    }

    // Update is called once per frame
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
        _audioSource.Play();

        yield return StartCoroutine(FadeIn(gameLogo, 1.0f));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOut(gameLogo, 1.0f));
        yield return new WaitForSeconds(2.0f);

        var creditList = new List<Tuple<string, string>>();
        creditList.Add(new Tuple<string, string>("PD / 프로그래밍", "주휘중"));
        creditList.Add(new Tuple<string, string>("기획", "김지후"));
        creditList.Add(new Tuple<string, string>("기획", "손지윤"));
        creditList.Add(new Tuple<string, string>("3D모델링 / 캐릭터 모델링", "류시영"));
        creditList.Add(new Tuple<string, string>("3D모델링 / 애니메이션", "이은수"));
        creditList.Add(new Tuple<string, string>("기술적 도움", "정종필"));

        for (int i = 0; i < creditList.Count; i++)
        {
            role.text = creditList[i].Item1;
            creditName.text = creditList[i].Item2;

            yield return StartCoroutine(FadeInTwoTexts(role, creditName, 1.0f));
            yield return new WaitForSeconds(2.0f);
            yield return StartCoroutine(FadeOutTwoTexts(role, creditName, 1.0f));
            yield return new WaitForSeconds(2.0f);
        }

        yield return StartCoroutine(FadeIn(engineLogo, 1.0f));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOut(engineLogo, 1.0f));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(FadeIn(schoolLogo, 1.0f));
        yield return StartCoroutine(FadeIn(description, 1.0f));
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(FadeOut(schoolLogo, 1.0f));
        yield return StartCoroutine(FadeOut(description, 1.0f));
        yield return new WaitForSeconds(2.0f);

        //54초에 나와야함
        yield return StartCoroutine(FadeIn(teamLogo, 1.0f));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOut(teamLogo, 1.0f));

        yield return new WaitForSeconds(2.0f);
        ovrScreenFade.FadeOut();
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);
        SceneManager.LoadScene("Game");
    }

    private IEnumerator FadeInTwoTexts(Text t1, Text t2, float duration)
    {
        Color color = t1.color;

        while (t1.color.a < 1f)
        {
            color.a += Time.deltaTime / duration;
            t1.color = color;
            t2.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOutTwoTexts(Text t1, Text t2, float duration)
    {
        Color color = t1.color;
        while (t1.color.a > 0f)
        {
            color.a -= Time.deltaTime / duration;
            t1.color = color;
            t2.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeIn(Image img, float duration)
    {
        Color color = img.color;

        while (img.color.a < 1f)
        {
            color.a += Time.deltaTime / duration;
            img.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeIn(Text text, float duration)
    {
        Color color = text.color;

        while (text.color.a < 1f)
        {
            color.a += Time.deltaTime / duration;
            text.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut(Image img, float duration)
    {
        Color color = img.color;

        while (img.color.a > 0f)
        {
            color.a -= Time.deltaTime / duration;
            img.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut(Text text, float duration)
    {
        Color color = text.color;

        while (text.color.a > 0f)
        {
            color.a -= Time.deltaTime / duration;
            text.color = color;
            yield return null;
        }
    }

    private IEnumerator ReturnToGameScene()
    {
        ovrScreenFade.FadeOut();
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);
        SceneManager.LoadScene("Game");
    }
}