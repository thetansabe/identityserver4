using System.Threading.Tasks;
using IdentityModel.Client;

namespace is4mvc
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}