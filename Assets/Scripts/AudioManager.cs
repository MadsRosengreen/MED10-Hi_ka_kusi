using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip[] audioClips;
    private AudioSource Lyd;

    [SerializeField] private int time2Intro = 30;
    [SerializeField] private int time2Join = 9;
    [SerializeField] private int time2Drop = 6;
    [SerializeField] private int time2Reset = 5;
    [SerializeField] private float voiceLineInterval = 0.5f;

    [SerializeField] private float hiKaKusiVolume = 0.5f;

    [SerializeField] private bool subOnOFf = true;

    private void Start()
    {
        Lyd = GetComponent<AudioSource>();
        Instance = this;
        StartCoroutine(ManageAudio());
    }
    public void StartAudio(int number, float db = 1)
    {
        Lyd.clip = audioClips[number];
        Lyd.Play();
        Lyd.loop = false;
        Lyd.volume = db;
    }

    public void IntroSub()
    {
        if(subOnOFf) GameManager.Instance.setText("\"It has been raining, the fields is growing, \n bearing fruits and we are happy. \n Enjoying ourselves.\"");
        AnimationFunctions.PlayAnimation(NPCManager.Instance.questAni, "Talk1", true);
        StartAudio(1);
    }

    public void ComeJoin()
    {
        if (subOnOFf) GameManager.Instance.setText("\"I am asking you to come join us\"");
        AnimationFunctions.PlayAnimation(NPCManager.Instance.questAni, "Talk2", true);
        StartAudio(2);
    }

    public void NeverDrop()
    {
        if (subOnOFf) GameManager.Instance.setText("\"Don't drop the 'tsama' (object), \n never drop it\"");
        AnimationFunctions.PlayAnimation(NPCManager.Instance.questAni, "Talk3", true);
        StartAudio(3);
    }

    public void ResetSub()
    {
        GameManager.Instance.setText("");
    }

    public IEnumerator stopHiKaKusi(float timeInSeconds)
    {
        AudioSource aud = Lyd.GetComponentInChildren<AudioSource>();
        while (aud.volume >= 0)
        {
            aud.volume -= timeInSeconds * Time.deltaTime;
            yield return null;
        }
        Lyd.GetComponentInChildren<AudioSource>().Stop();
    }

    public void StartHiKaKusi()
    {
        StartCoroutine(playHiKaKusi(1f));
    }

    public IEnumerator playHiKaKusi(float timeInSeconds)
    {
        AudioSource aud = Lyd.GetComponentInChildren<AudioSource>();
        aud.Play();
        aud.loop = true;
        while (aud.volume <= hiKaKusiVolume)
        {
            aud.volume -= timeInSeconds * Time.deltaTime;
            yield return null;
        }
        Lyd.GetComponentInChildren<AudioSource>().volume = hiKaKusiVolume;
    }

    public IEnumerator ManageAudio()
    {
        yield return new WaitForSecondsRealtime(time2Intro);
        IntroSub();
        yield return new WaitForSecondsRealtime(voiceLineInterval);
        AnimationFunctions.StopAnimationTransition(NPCManager.Instance.questAni, "Talk1");
        yield return new WaitForSecondsRealtime(time2Join);
        ComeJoin();
        yield return new WaitForSecondsRealtime(voiceLineInterval);
        AnimationFunctions.StopAnimationTransition(NPCManager.Instance.questAni, "Talk2");
        yield return new WaitForSecondsRealtime(time2Drop);
        NeverDrop();
        yield return new WaitForSecondsRealtime(voiceLineInterval);
        AnimationFunctions.StopAnimationTransition(NPCManager.Instance.questAni, "Talk3");
        yield return new WaitForSecondsRealtime(time2Reset);
        ResetSub(); 
        yield return null;
    }

    public IEnumerator ControllerText(bool whichText)
    {
        if(whichText) GameManager.Instance.setText("Controller activated");
        else GameManager.Instance.setText("Handtracking activated");
        yield return new WaitForSecondsRealtime(3);
        ResetSub();
        yield return null;
    }

    public IEnumerator WinLose(bool isWinning = false)
    {
        if (isWinning) GameManager.Instance.setText("You are the last person standing \n Congratulation!");
        else GameManager.Instance.setText("Unfortunately you have been eliminated  \n Feel free to try again!");
        yield return new WaitForSeconds(12);
        ResetSub();
        yield return null;
    }
}
