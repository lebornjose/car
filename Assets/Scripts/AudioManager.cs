using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioSource Audio_MouseClick;
    public AudioSource Audio_CarClick;
    public AudioSource CarBoom;
    public AudioSource CarWin;
    public AudioSource Audio_GetItem;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 通用播放方法（保持现有功能）
    public void PlayMouseClick()
    {
        Audio_MouseClick?.Play();
    }

    // 新增专用播放方法
    public void PlayCarClick()
    {
        Audio_CarClick?.Play();
    }

    public void PlayCarBoom()
    {
        CarBoom?.Play();
    }

    public void PlayCarWin()
    {
        CarWin?.Play();
    }
    public void PlayAudio_GetItem()
    {
        Audio_GetItem?.Play();
    }
}
