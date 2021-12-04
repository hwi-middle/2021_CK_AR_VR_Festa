using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneManager : MonoBehaviour
{
    [SerializeField] private OVRScreenFade ovrScreenFade;
    [SerializeField] private Text gameOverTitle;
    [SerializeField] private Text gameOverDialog;
    private AudioSource _audioSource;
    [SerializeField] private Image buttonHider;
    [SerializeField] private GameObject buttons;

    [SerializeField] private string[] dialogs;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        StartCoroutine(Proceed());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Proceed()
    {
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);
        Color color = gameOverTitle.color;
        float time1 = 2.0f;
        while (gameOverTitle.color.a < 1f)
        {
            color.a += Time.deltaTime / time1;
            gameOverTitle.color = color;
            yield return null;
        }

        string line = dialogs[Random.Range(0, 5)];

        float delay = 0.1f;
        yield return new WaitForSeconds(delay);
        foreach (var t in line)
        {
            gameOverDialog.text += t;
            _audioSource.Play();
            yield return new WaitForSeconds(delay);
        }
        
        buttons.SetActive(true);
        color = buttonHider.color;
        float time2 = 2.0f;
        while (buttonHider.color.a > 0f)
        {
            color.a -= Time.deltaTime / time2;
            buttonHider.color = color;
            yield return null;
        }
        
    }

    public void ReturnToGameSceneBtn()
    {
        StartCoroutine(ReturnToGameScene());
    }

    private IEnumerator ReturnToGameScene()
    {
        ovrScreenFade.FadeOut();
        yield return new WaitForSeconds(ovrScreenFade.fadeTime);
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}