namespace TheGamePond.Data;

public static class AppRoles
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string Staff = "Staff";

    public const string OwnerId = "1d4d24b0-83f5-42a0-9d92-441c49e026b5";
    public const string AdminId = "266e63c6-782a-48d1-a7c6-b1f91cd9e4bb";
    public const string StaffId = "8d16cdbb-5475-4570-aad9-f73b33ac5a3a";

    public static readonly string[] All = [Owner, Admin, Staff];
}
