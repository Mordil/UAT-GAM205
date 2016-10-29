using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Flags]
public enum PowerupAffectedEntities { Players = 1, Enemies = 2 }

public static class PowerupAffectedEntitiesExtensions
{
    public static bool Is(this PowerupAffectedEntities self, PowerupAffectedEntities typeToCheck)
    {
        return (self & typeToCheck) == typeToCheck;
    }

    public static bool Has(this PowerupAffectedEntities self, PowerupAffectedEntities typeToCheck)
    {
        return (self | typeToCheck) == typeToCheck;
    }
}

public interface IPowerup
{
    bool IsPermanent { get; }
    bool IsPickup { get; }

    float Duration { get; }

    void OnPickup(TankController controller);
    void OnUpdate(TankController controller);
    void OnExpire(TankController controller);
}

public static class IPowerupExtensions
{
    public static T CastAs<T>(this IPowerup self)
        where T : IPowerup
    {
        if (self is T)
        {
            return (T)(self);
        }

        return default(T);
    }
}
