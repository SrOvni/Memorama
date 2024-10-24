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
    AudioSource mainAudio;
    AudioSource sfx;
    public AudioSource SFX {get{return sfx;} set{sfx = value;}}
    public bool StartGame{get{return startGame;}set{startGame = value;}}
    private void Awake() {
        audioSources = GetComponents<AudioSource>();
        mainAudio = audioSources[0];
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
        mainAudio.Play();
    }
    
}
