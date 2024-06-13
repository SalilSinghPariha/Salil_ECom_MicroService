namespace Ecom_Web.Services.IService
{
    public interface ITokenProvider
    {
        void setToken(string token);
        string? getToken();

        void clearToken();
    }
}
