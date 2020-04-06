using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audiomanager : MonoBehaviour
{
    static Audiomanager audiomanager;
    static Dictionary<Sounds, AudioClip> audioClips =
    new Dictionary<Sounds, AudioClip>();

    [SerializeField]
    AudioClip Win;  
    [SerializeField]
    AudioClip Loose;  
    [SerializeField]
    AudioClip Drop;   
    [SerializeField]
    AudioClip Button;

    public enum Sounds
    {
        Win, Loose, Drop, Button
    }

    static AudioSource Audioplayer;

    // Start is called before the first frame update

    void Awake()
    {
        if (audiomanager == null)
        {
            DontDestroyOnLoad(this);
            audiomanager = this;
        }
        else
            Destroy(this);
    }


    void Start()
    {
        Audioplayer = GetComponent<AudioSource>();
        audioClips.Add(Sounds.Win, Win);
        audioClips.Add(Sounds.Loose, Loose);
        audioClips.Add(Sounds.Drop, Drop);
        audioClips.Add(Sounds.Button, Button);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(Sounds sounds)
    {
        AudioClip clip = null;
        audioClips.TryGetValue(sounds, out clip);
        Audioplayer.PlayOneShot(clip);
    }
}
