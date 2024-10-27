using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    
    [SerializeField] private int imagesRequired = 8;
    public int ImagesRequired{get{return imagesRequired;}}
    private bool startGame = false;
    AudioSource[] audioSources; 
    
    [SerializeField] AudioSource mainAudio;
    public AudioSource MainAudio{get{return mainAudio;}set{mainAudio = value;}}
    AudioSource sfx;
    public AudioSource SFX {get{return sfx;} set{sfx = value;}}
    [SerializeField] AudioClip mainAudioClip;
    public bool StartGame{get{return startGame;}set{startGame = value;}}
    private void Awake() {
        audioSources = GetComponents<AudioSource>();
        SFX = audioSources[1];
        if(Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
        
    }
    public void PlayMainAudio()
    {
        mainAudio.enabled = true;
        mainAudio.Play();
    }
    public void StopMainAudio()
    {
        mainAudio.Stop();
        mainAudio.enabled = false;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
