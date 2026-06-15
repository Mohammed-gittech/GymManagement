
using GymManagement.BLL.DTOs.SubscriptionPlans;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<IEnumerable<SubscriptionPlanResponseDto>> GetAllAsync();
        Task<SubscriptionPlanResponseDto> CreateAsync(CreateSubscriptionPlanDto dto);
        Task<SubscriptionPlanResponseDto> UpdateAsync(UpdateSubscriptionPlanDto dto, int id);
        Task DeleteAsync(int id);
    }
}