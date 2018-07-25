namespace DocumentExplorer.Infrastructure.Exceptions
{
    public static class ErrorCodes
    {
        public static string InvalidCredentials => "invalid_credentials";
        public static string UsernameInUse => "username_in_use";
        public static string UserNotFound => "user_not_found";
        public static string OrderNotFound => "order_not_found";
        public static string InvalidExtension => "invalid_extension";
        public static string NoFileName => "no_file_name";
        public static string NoFile => "no_file";
        public static string FileHasNoData => "file_has_no_data";
        public static string FileNotFound => "file_not_found";
        public static string InvalidFileType => "invalid_file_type";
    }
}
