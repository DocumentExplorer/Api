namespace DocumentExplorer.Core.Domain
{
    public static class ErrorCodes
    {
        public static string InvalidUsername => "invalid_username";
        public static string InvalidRole => "invalid_role";
        public static string IvalidId => "invalid_id";
        public static string InvalidCountry => "invalid_country";
        public static string InvalidNIP => "invalid_nip";
        public static string FileDoesNotExists => "file_does_not_exits";
        public static string FileTypeNotSpecified => "file_type_not_specified";
        public static string FileIsAlreadyAssigned => "file_is_already_assigned";
        public static string InvalidFileType => "invalid_file_type";
    }
}
