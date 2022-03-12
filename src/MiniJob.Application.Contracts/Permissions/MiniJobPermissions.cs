namespace MiniJob.Permissions
{
    public static class MiniJobPermissions
    {
        public const string GroupName = "MiniJob";

        public static class AppInfos
        {
            public const string Default = GroupName + ".AppInfos";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }

        public static class JobInfos
        {
            public const string Default = GroupName + ".JobInfos";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }
}