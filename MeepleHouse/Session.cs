namespace MeepleHouse
{
    public static class Session
    {
        public static Users CurrentUser { get; set; }

        public static Admins CurrentAdmin { get; set; }

        public static Workers CurrentWorker { get; set; }

        public static string CurrentRole { get; set; }
    }
}