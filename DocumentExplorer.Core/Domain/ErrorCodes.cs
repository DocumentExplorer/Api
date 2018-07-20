namespace DocumentExplorer.Core.Domain
{
    public static class ErrorCodes
    {
        public static string InvalidUsername => "invalid_username";
        public static string InvalidRole => "invalid_role";
        public static string IvalidId => "invalid_id";
        public static string InvalidCountry => "invalid_country";
        public static string InvalidNIP => "invalid_nip";
        public static string UserIsAleardyFirstOwner => "user_is_already_first_owner";
        public static string UserCannotChangeHisOwnOwnership => "user_cannot_change_his_own_ownership";
    }
}
