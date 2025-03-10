using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    [Space]
    [SerializeField] private float sfxMinHearableDistance;

    public bool playingBGM;
    private int bgmIndex;

    private bool canPlaySFX = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Invoke("AllowPlayingSFX", 0.2f);
    }

    private void Update()
    {
        if (!playingBGM)
        {
            StopAllBGM();
        }
        else
        {
            if (bgm[bgmIndex].isPlaying == false)
            {
                PlayBGM(bgmIndex);
            }
        }
    }

    public void PlaySFX(int _sfxIndex, Transform _sfxSourceTransform)
    {
        if (!canPlaySFX)
        {
            return;
        }

        if (sfx[_sfxIndex].isPlaying == true)
        {
            return;
        }

        if (_sfxSourceTransform != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _sfxSourceTransform.position) > sfxMinHearableDistance)
        {
            return;
        }


        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 1.15f);
            sfx[_sfxIndex].Play();
        }
    }

    public void StopSFX(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Stop();
        }
    }

    //public void StopSFXGradually(int _sfxIndex)
    //{
    //    if (sfx[_sfxIndex].isPlaying == false)
    //    {
    //        return;
    //    }

    //    StartCoroutine(DecreaseVolumeGradually(sfx[_sfxIndex]));
    //}

    public IEnumerator DecreaseVolumeGradually(AudioSource _audio)
    {
        float defaultVolume = _audio.volume;

        while (_audio.volume > 0.1f)
        {
            _audio.volume -= _audio.volume * 0.2f;

            yield return new WaitForSeconds(0.25f);

            if (_audio.volume <= 0.1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }

        }

    }

    public void PlayBGM(int _bgmIndex)
    {
        StopAllBGM();

        if (_bgmIndex < bgm.Length)
        {
            bgmIndex = _bgmIndex;
            bgm[bgmIndex].Play();
        }
        else
        {
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    public void AllowPlayingSFX()
    {
        canPlaySFX = true;
    }
}
