using UnityEngine;

public class SurfaceMaterial : MonoBehaviour
{
    public SurfaceType SurfaceType => materialSurfaceType;

    [SerializeField]
    private SurfaceType materialSurfaceType;
}
