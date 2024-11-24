using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//做了个静态类，随时随地调用就行。原理是Resources.Load加载音频
//并且动态创建GO添加AudioSouce组件然后播放
//我太懒了而且这个项目也用不着优化所以就没写对象池
//【重要】记得AUDIO_PATH设置音频路径
public static class SoundManager 
{
    public const string AUDIO_PATH = "Audios/Effects/";//音效路径
    private static GameObject oneShotObj;
    private static AudioSource oneShotAudioSource;
    private static GameObject defalutSoundObj_1;//没想好干啥,也许用来做BGM？可BGM直接在unity里面设置不就好了(没错我就是懒
    private static Dictionary<string,float> SfxName_PlayTime;//记录某sfx的上一次播放时间
    private static Dictionary<string,AudioClip> audios;//存储sfx资源
    public static Queue<string> currentClips = new Queue<string>();
    public static float playVolume = .7f;
    private static List<AudioSource> Sfx_multiOneShotAudioSourceList;
    private const int MAX_PLAY_CNT = 5;

    //播放音效直接调用这个函数就行
    //它不关心能否同一时间大量播放,即你每一帧调用一次的话它每一帧播放一次，它不关上一个同样的音效放没放完
    public static void PlayAudio(string _sfxName)
    {
        if(oneShotObj == null)
        {
            oneShotObj = new GameObject(AUDIO_PATH + "OneShotSound");
            oneShotAudioSource = oneShotObj.AddComponent<AudioSource>();
        }

        if (GetAudio(_sfxName))
        {
            oneShotAudioSource.volume = playVolume;
            oneShotAudioSource.PlayOneShot(GetAudio(_sfxName));

            currentClips.Enqueue(_sfxName);
        }
        else
        {
            Debug.LogWarning($"Audio {_sfxName} not found.");
        }
    }

    //播放音效直接调用这个函数就行
    //它关心能否同一时间大量播放
    public static void PlayOneAudio(string _sfxName)
    {
        if(SfxName_PlayTime == null)
        {
            SfxName_PlayTime = new Dictionary<string, float>();
        }

        if(!SfxName_PlayTime.ContainsKey(_sfxName))
        {
            SfxName_PlayTime.Add(_sfxName,0f);
        }
        float curTime = Time.time;
        AudioClip clip = GetAudio(_sfxName);
        //如果播放完
        if(curTime > SfxName_PlayTime[_sfxName] + clip.length)
        {
            if(oneShotObj == null)
            {
                oneShotObj = new GameObject(AUDIO_PATH + "OneShotSound");
                oneShotAudioSource = oneShotObj.AddComponent<AudioSource>();
            }
            oneShotAudioSource.volume = playVolume;
            oneShotAudioSource.PlayOneShot(GetAudio(_sfxName));
            SfxName_PlayTime[_sfxName] = curTime;
        }
    }

    public static void PlayAudioWithLimit(string _sfxName){
        if(oneShotObj == null)
        {
            oneShotObj = new GameObject(AUDIO_PATH + "OneShotSound");
            oneShotAudioSource = oneShotObj.AddComponent<AudioSource>();
        }

        AudioSource source = Sp_GetSfxAudioSource(_sfxName);
        if(source){
            if (GetAudio(_sfxName))
            {
                source.clip = GetAudio(_sfxName);
                source.volume = playVolume;
                source.Play();
                currentClips.Enqueue(_sfxName);
            }
            else
            {
                Debug.LogWarning($"Audio {_sfxName} not found.");
            }
        }
    }

    //3D音效，不过应该用不上
    public static void PlayAudio(string _sfxName,Vector3 pos)
    {
        GameObject soundObj = new GameObject("Sound");
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = GetAudio(_sfxName);
        soundObj.transform.position = pos;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.Play();

        Object.Destroy(soundObj,audioSource.clip.length);
    }

    //获取audio clip
    private static AudioClip GetAudio(string _sfxName)
    {
        if(audios == null)
        {
            audios = new Dictionary<string, AudioClip>();
        }
    
        if(!audios.ContainsKey(_sfxName))
        {
            AudioClip clip = Resources.Load<AudioClip>(AUDIO_PATH + _sfxName);
            audios.Add(_sfxName,clip);
        }
        return audios[_sfxName];
    }
    //一共MAX_PLAY_CNT个可供你随时调用的AudioSource
    private static AudioSource Sp_GetSfxAudioSource(string _sfxName){
        if(Sfx_multiOneShotAudioSourceList == null){
            Sfx_multiOneShotAudioSourceList = new List<AudioSource>();
            for (int i = 0; i < MAX_PLAY_CNT; i++)
            {
                if(oneShotObj == null)
                {
                    oneShotObj = new GameObject(AUDIO_PATH + "OneShotSound");
                    oneShotAudioSource = oneShotObj.AddComponent<AudioSource>();
                }
                Sfx_multiOneShotAudioSourceList.Add(oneShotObj.AddComponent<AudioSource>());
            }
        }

        foreach (var audio in Sfx_multiOneShotAudioSourceList)
        {
            if(!audio.isPlaying){
                return audio;
            }
        }

        return null;
    }
    //重新加载场景时
    public static void WhenSceneReload(){
        if(Sfx_multiOneShotAudioSourceList != null){
            Sfx_multiOneShotAudioSourceList.Clear();
            Sfx_multiOneShotAudioSourceList=null;
        }
        
    }

}
