using L4.Unity.Common.Audio;
using UnityEngine;

public class AudioSourceManager : AudioSourceExtenderBase
{
    protected override void Start()
    {
        base.Start();

        AudioSource.volume = GameManager.Instance.Settings.GetAudioLevelFromChannel(AudioChannel);
        GameManager.Instance.Settings.ChannelVolumeChanged += OnVolumeSourceChanged;
    }

    protected override void OnVolumeSourceChanged(object sender, ChannelVolumeChangedEventArgs e)
    {
        if (e.Channel == AudioChannel.Master ||
            e.Channel == AudioChannel)
        {
            AudioSource.volume = GameManager.Instance.Settings.GetAudioLevelFromChannel(AudioChannel);
        }
    }
}
