
namespace GymManagement.DAL.Services
{
    public interface ICurrentUserService
    {
        int? GetUserId();
        string? GetUsername();
        string? GetRole();
    }
}