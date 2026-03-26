namespace Expo.Domain.Constants;

/// <summary>
/// Constants deifning Endpoint policy
/// </summary>
public static class Policy
{
    /// <summary>
    /// USers policy
    /// </summary>
    public static class Users
    {
        /// <summary>
        /// Can create user
        /// </summary>
        public const string CanCreateUser = "CanCreateUser";
        /// <summary>
        /// Can promote user
        /// </summary>
        public const string CanPromoteUser = "CanPromoteUser";
        /// <summary>
        /// Can demote user
        /// </summary>
        public const string CanDemoteUser = "CanDemoteUser";
        /// <summary>
        /// Can read user
        /// </summary>
        public const string CanReadUser = "CanReadUser";
    }

    /// <summary>
    /// Entity policy
    /// </summary>
    public static class Entity
    {
        /// <summary>
        /// Can Create enity
        /// </summary>
        public const string CanCreateEntity = "CanCreateEntity";
        /// <summary>
        /// Can update entity
        /// </summary>
        public const string CanUpdateEntity = "CanUpdateEntity";
        /// <summary>
        /// Can delete entity
        /// </summary>
        public const string CanDeleteEntity = "CanDeleteEntity";
    }
}