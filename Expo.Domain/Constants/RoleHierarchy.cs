namespace Expo.Domain.Constants;

/// <summary>
/// Enum defining available roles
/// </summary>
public enum Role
{
    //Guest,
    //Operator,
    /// <summary>
    /// Supervisor
    /// </summary>
    Supervisor,
    /// <summary>
    /// Adiministrator
    /// </summary>
    Admin
}

/// <summary>
/// Utility class for managing roles
/// </summary>
public static class RoleHierarchy
{

    private static readonly Dictionary<Role, List<string>> _permissionsMap = new()
    {
        //{ Role.Guest, new List<string>() },

        //{ Role.Operator, new List<string> { Permissions.Entities.Create } },

        { Role.Supervisor, new List<string>
            { Permissions.Entities.Create,
              Permissions.Entities.Update,
              Permissions.Users.Read
            }
        },

        { Role.Admin, new List<string>
            {   Permissions.Entities.Delete,
                Permissions.Users.Create,
                Permissions.Users.Promote,
                Permissions.Users.Demote,
            }
        }
    };

    /// <summary>
    /// Return the lower role available
    /// </summary>
    /// <returns>Role</returns>
    public static Role GetMinRole()
    {
        return Enum.GetValues<Role>().Min();
    }
    /// <summary>
    /// Return the role name by enum
    /// </summary>
    /// <param name="role">Input role</param>
    /// <returns>Role name as string</returns>
    public static string GetRoleName(Role role) => role.ToString();
    /// <summary>
    /// Get permission linked to a role
    /// </summary>
    /// <param name="role">Inpu role</param>
    /// <returns>List of permission</returns>
    public static List<string> GetRolePermissions(Role role)
    {
        var cumulativePermissions = new List<string>();

        foreach (Role r in Enum.GetValues(typeof(Role)))
        {
            if (r <= role)
            {
                if (_permissionsMap.TryGetValue(r, out var perms))
                {
                    cumulativePermissions.AddRange(perms);
                }
            }
        }

        // Rimuove eventuali duplicati
        return [.. cumulativePermissions.Distinct()];
    }
    /// <summary>
    /// Get permission linked to a role
    /// </summary>
    /// <param name="roleName">Role name</param>
    /// <returns>List of permission</returns>
    public static List<string> GetRolePermissions(string roleName)
    {
        if (Enum.TryParse<Role>(roleName, true, out var role))
            return GetRolePermissions(role);

        return [];
    }
    /// <summary>
    /// Return role by name
    /// </summary>
    /// <param name="name">Role name</param>
    /// <returns>Role</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Role GetRoleByName(string name)
    {
        if (Enum.TryParse<Role>(name, true, out var role))
            return role;

        throw new ArgumentException($"Invalid role name: {name}", nameof(name));
    }
}