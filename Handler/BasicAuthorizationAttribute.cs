using Microsoft.AspNetCore.Authorization;

namespace webapi_ENVIA.Handler
{
    public class BasicAuthorizationAttribute : AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}
