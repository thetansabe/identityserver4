namespace IdentityServerHost.Quickstart.UI
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string ClientId { get; set; }
        public string? ClientName { get; set; }
        //public string ProtocolType { get; set; }
        //public bool RequireClientSecret { get; set; }
        //public bool RequireConsent { get; set; }
        //public bool AllowRememberConsent { get; set; }
        //public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        //public bool RequirePkce { get; set; }
        //public bool AllowPlainTextPkce { get; set; }
        //public bool RequireRequestObject { get; set; }
        //public bool AllowAccessTokensViaBrowser { get; set; }
        public List<RedirectUriViewModel> RedirectUris { get; set; }
    }

    public class RedirectUriViewModel
    {
        public string RedirectUri { get; set; }
    }
}
