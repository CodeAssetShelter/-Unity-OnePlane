using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active SoundManager!");
                }
            }
            return _instance;
        }
    }


    public enum BGM { Lobby = 0, Stage, Boss }
    public enum Speaker { Left = 0, Center, Right }
    public enum State { Play = 0, Pause, UnPause, Stop }
    public enum UISound { Button = 0, ChangePlane, Purchase, PurchaseFail, OverHeat }
    public AudioClip test;
    public AudioClip bgmLobby, bgmStage;

    [Header("- Volume")]
    public float bgmVolume = 1;
    public float effectVolume = 1;

    [Header("- Speakers")]
    public AudioSource left;
    public AudioSource center;
    public AudioSource right;
    public AudioSource speaker;
    public AudioSource warningSpeaker;
    private float bgmStageTimeline = 0;

    [Header("- UI Sounds")]
    public AudioClip uiSoundButtonDown;
    public AudioClip uiSoundChangePlane;
    public AudioClip uiSoundBuyPlane;
    public AudioClip uiSoundPurchaseFail;
    public AudioClip uiSoundOverHeat;

    // Start is called before the first frame update
    void Start()
    {
        //speaker.clip = bgmLobby;
        //speaker.Play();

        left.panStereo = -1.0f;
        right.panStereo = 1.0f;

        SetVolume(bgmVolume, effectVolume);

        //StartCoroutine(BgmManager());

    }

    IEnumerator BgmManager()
    {
        //while (true)
        //{
        //Debug.LogError(speaker.time);

        yield return null;
        //}
    }

    private void AllStop()
    {
       speaker.Stop();
    }

    private void AllPause()
    {
        speaker.Pause();
    }

    public void BgmSpeaker(BGM bgmMode, State bgmState, AudioClip clip = null)
    {
        if (bgmMode == BGM.Boss && clip == null)
        {
            return;
        }

        switch (bgmState)
        {
            case State.Play:
                {
                    //AllStop();
                    if (bgmMode == BGM.Boss)
                    {
                        bgmStageTimeline = speaker.time;
                        //Debug.Log(bgmStageTimeline);
                        speaker.clip = clip;
                    }
                    if (bgmMode == BGM.Lobby)
                    {
                        speaker.clip = bgmLobby;
                    }
                    if (bgmMode == BGM.Stage)
                    {
                        speaker.clip = bgmStage;
                    }

                    speaker.Play();
                }
                break;

            // Onle Used to Stage bgm
            case State.Pause:
                AllPause();
                speaker.clip = bgmStage;
                bgmStageTimeline = speaker.time;
                speaker.Pause();
                break;

            // Onle Used to Stage bgm
            case State.UnPause:
                AllStop();
                speaker.clip = bgmStage;
                speaker.time = bgmStageTimeline;
                speaker.Play();
                break;

            case State.Stop:
                AllStop();
                break;
        }
    }


    public void UiSpeaker(UISound type)
    {
        if (type == UISound.Button)
            center.PlayOneShot(uiSoundButtonDown, effectVolume);
        if (type == UISound.Purchase)
            center.PlayOneShot(uiSoundBuyPlane, effectVolume);
        if (type == UISound.ChangePlane)
            center.PlayOneShot(uiSoundChangePlane, effectVolume);
        if (type == UISound.PurchaseFail)
        {
            center.PlayOneShot(uiSoundPurchaseFail, effectVolume);
        }
        if (type == UISound.OverHeat)
        {
            warningSpeaker.PlayOneShot(uiSoundOverHeat, effectVolume);
        }
    }

    public void ShortSpeaker(Speaker speaker, AudioClip clip)
    {
        switch (speaker)
        {
            case Speaker.Left:
                left.PlayOneShot(clip, effectVolume);
                break;

            case Speaker.Center:
                    center.PlayOneShot(clip, effectVolume);
                break;

            case Speaker.Right:
                right.PlayOneShot(clip, effectVolume);
                break;
        }
    }

    public void SetVolume(float bgmVolume, float effectVolume)
    {
        this.bgmVolume = bgmVolume;
        this.effectVolume = effectVolume;

        left.volume = right.volume = center.volume = effectVolume;
        speaker.volume = bgmVolume;
    }
    //public void BgmSpeaker(AudioClip bgmClip)
    //{
    //    if (bgm.Exists(clip => clip == bgmClip))
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        AudioSource audio = new AudioSource();
    //        audio.clip = bgmClip;
    //        bgm.Add(audio);
    //        if (bgm.Count >= 1)
    //        {
    //            foreach(AudioSource source in bgm)
    //            {
    //                source.Pause();
    //            }
    //        }
    //        bgm[bgm.Count - 1].Play();
    //    }
    //}


    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        //left.PlayOneShot(test);
    //        //BgmSpeaker(BGM.Boss, State.Play, bgmLobby);
    //        //Time.timeScale = 0;
    //        ShortSpeaker(Speaker.Center, test);
    //    }
    //    if (Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        //speaker.PlayOneShot(test);
    //        //center.clip = test;
    //        //center.Play();
    //    }
    //    if (Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        //BgmSpeaker(BGM.Stage, State.UnPause);
    //        //Time.timeScale = 1.0f;
    //        center.PlayOneShot(test);
    //        //right.PlayOneShot(test);
    //    }
    //}
}
