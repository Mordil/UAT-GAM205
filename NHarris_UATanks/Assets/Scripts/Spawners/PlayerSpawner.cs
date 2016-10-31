using UnityEngine;

[AddComponentMenu("Spawners/Player Spawner")]
public class PlayerSpawner : SpawnerBase
{
    public override SpawnerType EntityType { get { return SpawnerType.Player; } }
}
