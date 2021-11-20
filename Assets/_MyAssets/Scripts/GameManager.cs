using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject mainScreenUIObject;
    [SerializeField] private Text versionText;
    [SerializeField] private GameObject inGameUIObject;
    [SerializeField] private GameObject scanner;
    [SerializeField] private NPC[] villains;
    
    // Start is called before the first frame update
    void Start()
    {
        versionText.text =  $"version: {Application.version}";
        scanner.SetActive(false);
        mainScreenUIObject.SetActive(true);
        inGameUIObject.SetActive(false);
        StartNewGame(); //에디터 상 테스트용
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetDown(InputManager.Button.Thumbstick, InputManager.Controller.RTouch))
        {
            InputManager.Recenter();
        }
    }

    public void StartNewGame()
    {
        SetInGameUIAndObjects();
        StartCoroutine(ProceedGame());
    }

    public void LoadGame()
    {
        // SetInGameUIAndObjects();
        throw new NotImplementedException();
    }

    private void SetInGameUIAndObjects()
    {
        scanner.SetActive(true);
        mainScreenUIObject.SetActive(false);
        inGameUIObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator ProceedGame()
    {
        villains[0].gameObject.SetActive(true);
        yield return null;
    }
}