namespace BlazorHost
{
    public static class AppConfig
    {
        private const string _apiBaseUrl = "https://localhost:7270";

        public static string ApiBaseUrl() => _apiBaseUrl;
    }


}
