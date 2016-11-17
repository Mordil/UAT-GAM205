using System.Linq;
using UnityEngine;

public class StealthTank : MonoBehaviour
{
    private void Awake()
    {
        // This object may not be created at start, so we do this during Awake
        GetComponentsInChildren<MeshRenderer>()
            .ToList()
            .ForEach(renderer => renderer.enabled = false);
    }
}
