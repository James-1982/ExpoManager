namespace Expo.Domain.Constants
{
/// <summary>
/// Constants defining permissions
/// </summary>
public static class Permissions
{
    public static class Users
    {
        public const string Create = "users.create";
        public const string Promote = "users.promote";
        public const string Demote = "users.demote";
        public const string Read = "users.read";
    }

    public static class Entities
    {
        public const string Create = "entities.create";
        public const string Update = "entities.update";
        public const string Delete = "entities.delete";
    }
}
}