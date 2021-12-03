using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    [SerializeField] private OVRScreenFade ovrScreenFade;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
