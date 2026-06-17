using System.Threading.Tasks;
using GymManagement.BLL.DTOs.SubscriptionPlans;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.UnitOfWork;
using NSubstitute;

namespace GymManagement.Tests.Services
{
    public class SubscriptionPlanServiceTests
    {
        // Mocks
        private readonly IUnitOfWork _unitOfWork;

        // Services 
        private readonly ISubscriptionPlanService _subscriptionPlan;

        public SubscriptionPlanServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _subscriptionPlan = new SubscriptionPlanService(_unitOfWork);
        }

        // Get All
        [Fact]
        public async Task GetAllAsync_ShouldReturnsAllSubscriptionPlan()
        {
            // Arrange
            var subscriptionPlans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan
                {
                    Id =1,
                    Name = "plan1",
                    Price = 10000,
                    DurationDays = 30
                },
                new SubscriptionPlan
                {
                    Id =2,
                    Name = "plan2",
                    Price = 10000,
                    DurationDays = 30
                },
                new SubscriptionPlan
                {
                    Id =3,
                    Name = "plan3",
                    Price = 10000,
                    DurationDays = 30
                },
            };

            _unitOfWork.SubscriptionPlans
                .GetAllAsync()
                .Returns(subscriptionPlans);

            // Act
            var result = await _subscriptionPlan.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        // Get By ID
        [Fact]
        public async Task GetByIdAsync_WithExistingSubscriptionPlan_ReturnsSubscriptionPlanResponseDto()
        {
            // Arrange 
            var plan = new SubscriptionPlan
            {
                Id = 1,
                Name = "Plan",
                Price = 25000,
                DurationDays = 30
            };

            _unitOfWork.SubscriptionPlans
                .GetByIdAsync(1)
                .Returns(plan);

            // Act
            var result = await _subscriptionPlan.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(25000, result.Price);
            Assert.NotEmpty(result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingSubscriptionPlan_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWork.SubscriptionPlans
                .GetByIdAsync(Arg.Any<int>())
                .Returns((SubscriptionPlan?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _subscriptionPlan.GetByIdAsync(1));
        }

        // Create
        [Fact]
        public async Task CreateAsync_ShouldReturnSubscriptionPlanResponseDto()
        {
            // Arrange
            _unitOfWork.SubscriptionPlans
                .GetByNameAsync(Arg.Any<string>())
                .Returns((SubscriptionPlan?)null);

            _unitOfWork.SubscriptionPlans
                .When(x => x.Add(Arg.Any<SubscriptionPlan>()))
                .Do(x => x.Arg<SubscriptionPlan>().Id = 1);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);

            var plan = new CreateSubscriptionPlanDto
            {
                Name = "Plan",
                Price = 10000,
                DurationDays = 30
            };

            // Act
            var result = await _subscriptionPlan.CreateAsync(plan);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.NotEmpty(result.Name);
        }

        [Fact]
        public async Task CreateAsync_WithExistingSubscriptionPlan_ThrowsValidationException()
        {
            // Arrange
            var plan = new SubscriptionPlan
            {
                Id = 1,
                Name = "Plan",
                Price = 10000,
                DurationDays = 30
            };

            _unitOfWork.SubscriptionPlans
                .GetByNameAsync("Plan")
                .Returns(plan);

            var dto = new CreateSubscriptionPlanDto
            {
                Name = "Plan",
                Price = 10000,
                DurationDays = 30
            };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _subscriptionPlan.CreateAsync(dto));
        }

        // Update
        [Fact]
        public async Task UpdateAsync_ShouldReturnSubscriptionPlanResponseDto()
        {
            // Arrange
            var plan = new SubscriptionPlan
            {
                Id = 1,
                Name = "Plan",
                Price = 10000,
                DurationDays = 30
            };

            _unitOfWork.SubscriptionPlans
                .GetByIdTrackedAsync(1)
                .Returns(plan);

            _unitOfWork.SubscriptionPlans
                .GetByNameAsync("Plan")
                .Returns(plan);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);

            var dto = new UpdateSubscriptionPlanDto
            {
                Name = "Plan",
                Price = 20000,
                DurationDays = 30
            };

            // Act 
            var result = await _subscriptionPlan.UpdateAsync(dto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(20000, result.Price);
            Assert.Equal(30, result.DurationDays);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingSubscriptionPlan_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWork.SubscriptionPlans
                .GetByIdTrackedAsync(Arg.Any<int>())
                .Returns((SubscriptionPlan?)null);

            var dto = new UpdateSubscriptionPlanDto
            {
                Name = "Plan",
                Price = 20000,
                DurationDays = 30
            };

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _subscriptionPlan.UpdateAsync(dto, 1));
        }

        [Fact]
        public async Task UpdateAsync_WithExistingSubscriptionPlanAndDifferentId_ThrowsValidationException()
        {
            // Arrange
            var plan = new SubscriptionPlan
            {
                Id = 1,
                Name = "Plan",
                Price = 10000,
                DurationDays = 30
            };
            var plan2 = new SubscriptionPlan
            {
                Id = 2,
                Name = "Plan2",
                Price = 10000,
                DurationDays = 30
            };

            _unitOfWork.SubscriptionPlans
                .GetByIdTrackedAsync(1)
                .Returns(plan);

            _unitOfWork.SubscriptionPlans
                .GetByNameAsync("Plan2")
                .Returns(plan2);

            var dto = new UpdateSubscriptionPlanDto
            {
                Name = "Plan2",
                Price = 20000,
                DurationDays = 30
            };

            // Act + Assert 
            await Assert.ThrowsAsync<ValidationException>(() => _subscriptionPlan.UpdateAsync(dto, 1));
        }

        // Delete
        [Fact]
        public async Task DeleteAsync_WithExistingSubscrptionPlan_DeletesSuccessfully()
        {
            // Arrange
            var plan = new SubscriptionPlan
            {
                Id = 1,
                Name = "Plan",
                Price = 10000,
                DurationDays = 30,
            };

            _unitOfWork.SubscriptionPlans
                .GetByIdTrackedAsync(1)
                .Returns(plan);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);


            // Act
            await _subscriptionPlan.DeleteAsync(1);

            // Assert
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingSubscriptionPlan_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWork.SubscriptionPlans
                .GetByIdTrackedAsync(Arg.Any<int>())
                .Returns((SubscriptionPlan?)null);



            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _subscriptionPlan.DeleteAsync(1));
        }
    }
}