namespace Google.Apis.Auth.OAuth2.Flows
{
    public enum AccessType
    {
        Online,
        Offline
    }
    public class GoogleOauthParams
    {
        public string LoginHint { get; set; }
        public AccessType AccessType { get; set; }

        public GoogleOauthParams()
        {
        }
    }
}
