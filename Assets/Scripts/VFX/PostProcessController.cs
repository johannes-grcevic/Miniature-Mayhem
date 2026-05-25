using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [SerializeField]
    private Volume volume;

    public void ChangeDepthOfFieldState(GameState state)
    {
        if (volume.profile.TryGet(out DepthOfField depthOfField))
        {
            depthOfField.active = state == GameState.Paused || state == GameState.Win || state == GameState.Over;
        }
    }
}
