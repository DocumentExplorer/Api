namespace DocumentExplorer.Infrastructure.Exceptions
{
    public static class ErrorCodes
    {
        public static string InvalidCredentials => "invalid_credentials";
        public static string UsernameInUse => "username_in_use";
        public static string UserNotFound => "user_not_found";
        public static string OrderIdInUse => "order_id_in_use";
    }
}
