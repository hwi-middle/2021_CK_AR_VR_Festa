using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : NPC
{
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private GameObject blakOutQuad;
    private Renderer _blackOutQuadRenderer;
    private bool hit = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        PosSystem.currentState = POSSystem.EProceedState.None;
        StartCoroutine(Act());
    }

    private IEnumerator Act()
    {
        yield return StartCoroutine(GoToSpot(12));

        yield return StartCoroutine(StartNextDialog(7));
        StartCoroutine(FadeOutBgm(3.0f));
        yield return StartCoroutine(StartNextDialog(11));
        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.Play();
        yield return StartCoroutine(StartNextDialog(22));

        var pickInstance = Instantiate(pick);
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(StartNextDialog(2));

        for (int i = 0; i < 7; i++)
        {
            hit = false;
            while (!hit)
            {
                yield return null;
            }

            yield return StartCoroutine(StartNextDialog(1));
        }
        
        hit = false;
        while (!hit)
        {
            yield return null;
        }

        _blackOutQuadRenderer = blakOutQuad.GetComponent<Renderer>();
        _blackOutQuadRenderer.material.color = new Color(0, 0, 0, 1f);
        yield return new WaitForSeconds(3.0f);
    }

    private IEnumerator FadeOutBgm(float duration)
    {
        float vol = 1;
        while (bgmAudioSource.volume > 0f)
        {
            vol -= Time.deltaTime / duration;
            bgmAudioSource.volume = vol;
            yield return null;
        }

        bgmAudioSource.volume = 0f;
        bgmAudioSource.Stop();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        hit = true;
    }
}