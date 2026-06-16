
using GymManagement.BLL.DTOs.SubscriptionPlans;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.UnitOfWork;

namespace GymManagement.BLL.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubscriptionPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SubscriptionPlanResponseDto>> GetAllAsync()
        {
            // Get all subscription plans
            var plans = await _unitOfWork.SubscriptionPlans.GetAllAsync();
            // Convert to DTOs
            return plans.Select(p => p.ToDto());
        }
        public async Task<SubscriptionPlanResponseDto> CreateAsync(CreateSubscriptionPlanDto dto)
        {
            // Check if name already exists
            var existstingPlan = await _unitOfWork.SubscriptionPlans
                .GetByNameAsync(dto.Name);

            if (existstingPlan != null)
                throw new ValidationException("اسم الباقة موجود مسبقاً");

            // Create new plan
            var plan = new SubscriptionPlan
            {
                Name = dto.Name,
                Price = dto.Price,
                DurationDays = dto.DurationDays
            };

            // Save plan
            _unitOfWork.SubscriptionPlans.Add(plan);
            await _unitOfWork.SaveChangesAsync();

            // Convert to dtos
            return plan.ToDto();
        }

        public async Task<SubscriptionPlanResponseDto> UpdateAsync(UpdateSubscriptionPlanDto dto, int id)
        {
            // Get plan by id
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdTrackedAsync(id);
            if (plan == null)
                throw new NotFoundException("الباقة غير موجودة");

            // Check if name is already exists 
            var existstingPlan = await _unitOfWork.SubscriptionPlans.GetByNameAsync(dto.Name);
            if (existstingPlan != null && existstingPlan.Id != id)
                throw new ValidationException("اسم الباقة موجود مسبقاً");

            // Update plan
            plan.Name = dto.Name;
            plan.Price = dto.Price;
            plan.DurationDays = dto.DurationDays;

            // Save Changes 
            _unitOfWork.SubscriptionPlans.Update(plan);
            await _unitOfWork.SaveChangesAsync();

            // Return response
            return plan.ToDto();
        }
        public async Task DeleteAsync(int id)
        {
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdTrackedAsync(id);
            if (plan == null)
                throw new NotFoundException("الباقة غير موجودة");

            // Soft Delete
            _unitOfWork.SubscriptionPlans.Delete(plan);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}