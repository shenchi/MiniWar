using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audios;

    public static AudioManager Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;
    }

    void OnDisable()
    {
        Instance = null;
    }

    public void Play(int index)
    {
        if (null == audios || index < 0 || index >= audios.Length)
            return;

        audios[index].Play();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
