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
    private POSSystem _posSystem;
    [SerializeField] private Image[] lifes;
    private int _life = 3;
    private static readonly Color ActivatedLifeColor = new Color(1f, 0.4526f, 0f);
    private static readonly Color DeactivatedLifeColor = new Color(1f, 1f, 1f);
    private AudioSource _audioSource;
    [SerializeField] private OVRScreenFade ovrScreenFade;
    [SerializeField] private GameObject damageQuad;
    private Renderer _damageQuadRenderer;

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
            GameObject go = GameObject.FindWithTag("GameController");
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
        if (OVRPlugin.userPresent)
        {
            InputManager.Recenter();
        }

        _posSystem = POSSystem.Instance;
        versionText.text = $"version: {Application.version}";
        scanner.SetActive(false);
        mainScreenUIObject.SetActive(true);
        inGameUIObject.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
        _damageQuadRenderer = damageQuad.GetComponent<Renderer>();
        StartNewGame(); //에디터 상 테스트용
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
            ovrScreenFade.FadeOut();
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
                _posSystem.currentState = POSSystem.EProceedState.None;
                villains[npcIdx++].gameObject.SetActive(false);
                _posSystem.ResetGoodsAndRefresh();

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

    public void DecreaseLife()
    {
        if (_life <= 0) return;
        lifes[--_life].color = DeactivatedLifeColor;
        StartCoroutine(Damage(1f));
        _audioSource.Play();
    }

    private IEnumerator Damage(float t)
    {
        Color color = new Color(1, 0, 0, 0.7f);
        _damageQuadRenderer.material.color = color;

        while (_damageQuadRenderer.material.color.a > 0f)
        {
            color.a -= Time.deltaTime / t;
            _damageQuadRenderer.material.color = color;
            yield return null;
        }
    }
}