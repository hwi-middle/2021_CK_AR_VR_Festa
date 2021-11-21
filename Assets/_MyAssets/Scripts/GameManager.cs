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
    private POSSystem PosSystem;
    [SerializeField] private GameObject[] lifes;
    private int _life = 3;
    public int Life { get; set; }

    private static GameManager _instance;

    public static GameManager Instance
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
            GameObject go = GameObject.FindWithTag("GameManager");
            if (go == null)
            {
                Debug.LogError("GameManager not found");
            }

            _instance = go.GetComponent<GameManager>();
        }
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        PosSystem = POSSystem.Instance;
        versionText.text = $"version: {Application.version}";
        scanner.SetActive(false);
        mainScreenUIObject.SetActive(true);
        inGameUIObject.SetActive(false);
        // StartNewGame(); //에디터 상 테스트용
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetDown(InputManager.Button.Thumbstick, InputManager.Controller.RTouch))
        {
            InputManager.Recenter();
        }

        if (_life <= 0)
        {
            //게임오버 처리
        }
    }

    public void StartNewGame()
    {
        SetInGameUIAndObjects();
        StartCoroutine(ProceedGame(0));
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

    private IEnumerator ProceedGame(int npcIdx)
    {
        villains[npcIdx].gameObject.SetActive(true);

        while (!villains[villains.Length - 1].IsFinished)
        {
            if (villains[npcIdx].IsFinished)
            {
                PosSystem.currentState = POSSystem.EProceedState.None;
                villains[npcIdx++].gameObject.SetActive(false);
                PosSystem.ResetGoods();

                if (npcIdx < villains.Length)
                {
                    villains[npcIdx].gameObject.SetActive(true);
                }
            }
            else
            {
                yield return null;
            }
        }

        yield return null;
    }
}