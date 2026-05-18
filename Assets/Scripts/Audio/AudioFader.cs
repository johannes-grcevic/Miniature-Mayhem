using UnityEngine;

public static class AudioFader
{
    public static async Awaitable FadeVolumeAsync(AudioSource source, float targetVolume, float duration)
    {
        if (source == null) return;

        float startVolume = source.volume;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Stop processing if the AudioSource is destroyed mid-fade
            if (source == null) return;

            timeElapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);

            // Suspends execution until the next frame
            await Awaitable.EndOfFrameAsync();
        }

        if (source != null)
        {
            source.volume = targetVolume;

            if (targetVolume == 0f)
            {
                source.Stop();
            }
        }
    }
}
