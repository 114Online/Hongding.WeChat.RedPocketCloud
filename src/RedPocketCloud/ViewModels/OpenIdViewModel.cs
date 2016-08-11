using System;

namespace RedPocketCloud.ViewModels
{
    public class OpenIdViewModel
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpire { get; set; }
        public DateTime RefreshTokenExpire { get; set; }
        public string NickName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
