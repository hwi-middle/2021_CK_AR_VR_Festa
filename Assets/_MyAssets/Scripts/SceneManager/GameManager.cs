using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    [SerializeField] private float timeScaleForDebug = 1f;
    [SerializeField] private GameObject mainScreenUIObject;
    [SerializeField] private Text versionText;
    [SerializeField] private GameObject inGameUIObject;
    [SerializeField] private GameObject scanner;
    [SerializeField] private NPC[] villains;
    private POSSystem _posSystem;
    [SerializeField] private Text dialogText;
    [SerializeField] private Image[] lives;
    private int _life = 3;
    private static readonly Color ActivatedLifeColor = new Color(0f, 1f, 0.3728263f);
    private static readonly Color DeactivatedLifeColor = new Color(1f, 1f, 1f);
    private AudioSource _audioSource;
    [SerializeField] private OVRScreenFade ovrScreenFade;
    [SerializeField] private GameObject damageQuad;
    private Renderer _damageQuadRenderer;

    public AudioClip[] maleVoiceClips;
    public AudioClip[] oldMaleVoiceClips;
    public AudioClip[] femaleVoiceClips;
    public AudioClip[] hitClips;


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

        if (debugMode)
        {
            StartNewGame(); //????????? ??? ????????????
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode)
        {
            Time.timeScale = timeScaleForDebug;
        }

        if (InputManager.GetDown(InputManager.Button.Thumbstick, InputManager.Controller.RTouch))
        {
            InputManager.Recenter();
        }
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("Level", 0);
        SetInGameUIAndObjects();
        StartCoroutine(ProceedGame(0));
    }

    public void LoadGame()
    {
        SetInGameUIAndObjects();
        StartCoroutine(ProceedGame(PlayerPrefs.GetInt("Level", 0)));
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
                while (!villains[npcIdx].IsNavMeshAgentReachedDestination())
                {
                    yield return null;
                }

                dialogText.text = "";
                villains[npcIdx++].gameObject.SetActive(false);
                _posSystem.ResetGoodsAndRefresh();
                PlayerPrefs.SetInt("Level", npcIdx);

                if (npcIdx < villains.Length)
                {
                    yield return new WaitForSeconds(2.0f);
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
        _posSystem.ClearChangeText();

        lives[--_life].color = DeactivatedLifeColor;
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

        if (_life <= 0)
        {
            NPC.LoadedStaticObjects = false;
            ovrScreenFade.FadeOut();
            yield return new WaitForSeconds(ovrScreenFade.fadeTime);
            SceneManager.LoadScene("GameOver");
        }
    }
}