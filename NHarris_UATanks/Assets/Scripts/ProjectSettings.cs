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
        public const string PATROL_POINT = "PatrolPoint";
    }

    public static class Axes
    {
        public const string PLAYER1_VERTICAL = "Vertical_P1";
        public const string PLAYER1_HORIZONTAL = "Horizontal_P1";
        public const string PLAYER1_SHOOT = "Fire_P1";

        public const string PLAYER2_VERTICAL = "Vertical_P2";
        public const string PLAYER2_HORIZONTAL = "Horizontal_P2";
        public const string PLAYER2_SHOOT = "Fire_P2";

        public const string PLAYER3_VERTICAL = "Vertical_P3";
        public const string PLAYER3_HORIZONTAL = "Horizontal_P3";
        public const string PLAYER3_SHOOT = "Fire_P3";

        public const string PLAYER4_VERTICAL = "Vertical_P4";
        public const string PLAYER4_HORIZONTAL = "Horizontal_P4";
        public const string PLAYER4_SHOOT = "Fire_P4";
    }

    /// <summary>
    /// Project defined layer names and their backing ID.
    /// </summary>
    public enum Layers : int
    {
        Default = 0,
        Projectiles = 8,
        Player = 9,
        Enemy = 10,
        Floor = 11,
        Wall = 12,
        InvisiWall = 13,
        Powerup = 14,
        AIListener = 15,
        Door = 16
    }

    public enum Levels : int
    {
        MainMenu = 0,
        MainGameplay = 1
    }
}
