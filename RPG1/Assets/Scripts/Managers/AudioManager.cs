using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    //玩家听力距离
    [SerializeField] private float sfxMinimumDistance;
    //声音特效
    [SerializeField] private AudioSource[] sfx;
    //背景音
    [SerializeField] private AudioSource[] bgm;

    public bool playBGM;
    private int bgmIndex;

    private bool canPlaySFX;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        Invoke("AllowSFX", 1);
    }

    private void Update()
    {
        if (!playBGM)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                bgm[bgmIndex].Play();
            }
        }
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        if (!canPlaySFX)
            return;

        if (sfx[_sfxIndex].isPlaying)
            return;
        //如果声源距离玩家太远，则不播放声音特效
        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;

        if(_sfxIndex < sfx.Length)
        {
            //让每次特效产生细微的不同
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.1f);
            sfx[_sfxIndex].Play();

        }
    }

    public void StopSFX(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
            sfx[_sfxIndex].Stop();
    }

    public void PlayBGM(int _bgmIndex)
    {
        //先停止所有背景音乐的播放，再播放指定的背景音乐
        StopAllBGM();

        if (bgmIndex < bgm.Length)
        {
            bgmIndex = _bgmIndex;
            bgm[bgmIndex].Play();
        }
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void AllowSFX() => canPlaySFX = true;


    public void StopWithTime(int _index) => StartCoroutine(DecreaseVolumn(sfx[_index]));
    private IEnumerator DecreaseVolumn(AudioSource _audio)
    {
        //实现声音的非线性衰退过程
        float defaultVolumn = _audio.volume;

        while(_audio.volume > .0001f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.25f);

            if(_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolumn;
                break;
            }
        }
    }
}
