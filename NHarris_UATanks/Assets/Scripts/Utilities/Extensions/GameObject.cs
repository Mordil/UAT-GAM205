using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Compares the GameObject's layer int against the provided value.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="layerToCompare"></param>
    /// <returns>True if both are the same layer.</returns>
    public static bool IsOnSameLayer(this GameObject self, ProjectSettings.Layers layerToCompare)
    {
        return self.layer == (int) layerToCompare;
    }
}
