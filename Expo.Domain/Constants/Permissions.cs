namespace Expo.Domain.Constants;

/// <summary>
/// COnstant defining Permissions
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Users permission
    /// </summary>
    public static class Users
    {
        /// <summary>
        /// Can create user
        /// </summary>
        public const string Create = "users.create";
        /// <summary>
        /// Can promote user
        /// </summary>
        public const string Promote = "users.promote";
        /// <summary>
        /// Can demote user
        /// </summary>
        public const string Demote = "users.demote";
        /// <summary>
        /// Can read users
        /// </summary>
        public const string Read = "users.read";
    }

    /// <summary>
    /// Data permissions
    /// </summary>
    public static class Entities
    {
        /// <summary>
        /// Can create new entity
        /// </summary>
        public const string Create = "entity.create";
        /// <summary>
        /// Can update entity
        /// </summary>
        public const string Update = "entity.update";
        /// <summary>
        /// Can delete entity
        /// </summary>
        public const string Delete = "entity.delete";
    }
}
