namespace Expo.Domain.Constants
{
/// <summary>
/// Constants defining endpoint policies
/// </summary>
public static class Policy
{
    public static class Users
    {
        public const string CanCreateUser = "CanCreateUser";
        public const string CanPromoteUser = "CanPromoteUser";
        public const string CanDemoteUser = "CanDemoteUser";
        public const string CanReadUser = "CanReadUser";
    }

    public static class Entity
    {
        public const string CanCreateEntity = "CanCreateEntity";
        public const string CanUpdateEntity = "CanUpdateEntity";
        public const string CanDeleteEntity = "CanDeleteEntity";
    }
}
}