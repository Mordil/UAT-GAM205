using UnityEngine;

[AddComponentMenu("Spawners/Player Spawner")]
public class PlayerSpawner : SpawnerBase
{
    // This doesn't have much special logic - just need to specify what type it is for when attaching to gameobjects for prefabs

    public override SpawnerType EntityType { get { return SpawnerType.Player; } }
}
