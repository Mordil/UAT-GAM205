using L4.Unity.Common.Extensions;
using UnityEngine;
using System.Collections;

public class SequenceAudioSourceManager : AudioSourceManager
{
    private const float THRESHOLD_TO_SCHEDULE_NEXT_CLIP = 10f;
    
    private int _clipIndex;
    [SerializeField]
    private float _delayBetweenClips = 2f;

    [ReadOnly]
    [SerializeField]
    private AudioClip _nextClip;

    [SerializeField]
    private AudioClip[] _audioClips;

    protected override void Awake()
    {
        base.Awake();

        if (_audioClips.Length > 0)
        {
            AudioSource.clip = _audioClips[0];
            AudioSource.Play();
        }
    }

    protected override void Update()
    {
        base.Update();

        // if there are no clips, return
        if (_audioClips.Length == 0)
        {
            return;
        }

        // if the audio source has finished the last clip, set the next one
        if (!AudioSource.isPlaying)
        {
            PlayScheduledClip();
            SetNextClip();
        }
    }

    private void PlayScheduledClip()
    {
        AudioSource.SwitchAndStartClip(_nextClip, true, Mathf.RoundToInt(_delayBetweenClips));
    }

    private void SetNextClip()
    {
        // sets the next index and does index looping logic
        IncreaseIndex();

        // otherwise, set the scheduled next clip
        _nextClip = _audioClips[_clipIndex];
    }

    private void IncreaseIndex()
    {
        _clipIndex++;

        if (_clipIndex >= _audioClips.Length)
        {
            _clipIndex = 0;
        }
    }
}
