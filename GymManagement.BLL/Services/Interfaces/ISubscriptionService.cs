using GymManagement.BLL.DTOs.Subscription;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionResponseDto>> GetByMemberIdAsync(int memberId);
        Task<SubscriptionResponseDto> CreateAsync(CreateSubscriptionDto dto);
    }
}