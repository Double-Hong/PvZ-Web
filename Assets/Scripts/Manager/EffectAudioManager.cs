using System.Collections.Generic;
using UnityEngine;
using myh;
/// <summary>
/// 管理所有效果音的播放（不含背景音乐）
/// </summary>
public class EffectAudioManager : MonoSingleton<EffectAudioManager>
{
    // 音效资源缓存
    private Dictionary<string, AudioClip> mAudioClipCache = new();

    // AudioSource池
    private List<AudioSource> mAudioSources = new();

    // 最大同时音效播放数（避免无限增长）
    private const int MaxSources = 10;

    protected override void Init()
    {
        var newSource = gameObject.AddComponent<AudioSource>();
        mAudioSources.Add(newSource);
    }

    protected override void ResetData()
    {
        Debug.Log("EffectAudioManager 重置");
        // 可清空缓存或重置音效状态
        // audioClipCache.Clear();
    }

    protected override void Destroy()
    {
        Debug.Log("EffectAudioManager 销毁");
        mAudioClipCache.Clear();
        mAudioSources.Clear();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clipName">音效路径（Resources 相对路径）</param>
    public void PlayEffect(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return;

        // 尝试从缓存中获取
        if (!mAudioClipCache.TryGetValue(clipName, out var clip))
        {
            clip = Resources.Load<AudioClip>(clipName);
            if (clip == null)
            {
                Debug.LogWarning($"未找到音效资源：{clipName}");
                return;
            }
            mAudioClipCache[clipName] = clip;
        }

        var source = GetAvailableSource();
        source.PlayOneShot(clip);
    }

    /// <summary>
    /// 获取一个可用的 AudioSource
    /// </summary>
    private AudioSource GetAvailableSource()
    {
        // 查找未播放的
        foreach (var source in mAudioSources)
        {
            if (!source.isPlaying)
                return source;
        }

        // 没有可用的，创建一个（限制总数）
        if (mAudioSources.Count < MaxSources)
        {
            var newSource = gameObject.AddComponent<AudioSource>();
            mAudioSources.Add(newSource);
            return newSource;
        }

        // 全在播，返回第一个（可能会打断正在播放的）
        return mAudioSources[0];
    }
}
