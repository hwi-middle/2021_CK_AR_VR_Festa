using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private OVRScreenFade ovrScreenFade;
    [SerializeField] private float delay;

    // Start is called before the first frame update
    void Awake()
    {
        if (OVRPlugin.userPresent)
        {
            InputManager.Recenter();
        }

        StartCoroutine(MoveToMainScene());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator MoveToMainScene()
    {
        yield return new WaitForSeconds(delay);
        ovrScreenFade.FadeOut();
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);
        SceneManager.LoadScene("Game");
    }
}