public static class ProjectSettings
{
    public static class Tags
    {
        public const string BULLET = "Bullet";
    }

    public enum Layers : int
    {
        Projectiles = 8,
        Player = 9,
        Enemy = 10,
        Floor = 11,
        Wall = 12,
        InvisiWall = 13
    }
}
