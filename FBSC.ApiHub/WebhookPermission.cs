namespace FBSC.ApiHub
{
    public static class WebhookPermission
    {
        public static class WebhookApi
        {
            public const string View = "P.W.V";
            public const string Create = "P.W.C";
            public const string Edit = "P.W.E";
            public const string History = "P.W.H";
            public const string Logs = "P.W.L";
        }
        public static class WebhookLogs
        {
            public const string View = "P.Wl.V";
        }

        public static class WebhookEventLogs
        {
            public const string View = "P.We.V";
        }
    }
}
