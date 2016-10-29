/// <summary>
/// Static container for Project development settings or configurations.
/// </summary>
public static class ProjectSettings
{
    /// <summary>
    /// Static lookup for project defined object tags by name.
    /// </summary>
    public static class Tags
    {
        public const string BULLET = "Bullet";
    }

    /// <summary>
    /// Project defined layer names and their backing ID.
    /// </summary>
    public enum Layers : int
    {
        Projectiles = 8,
        Player = 9,
        Enemy = 10,
        Floor = 11,
        Wall = 12,
        InvisiWall = 13,
        Powerup = 14
    }
}
