using GymManagement.BLL.DTOs.Subscription;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.UnitOfWork;
using NSubstitute;

namespace GymManagement.Tests.Services
{
    public class SubscriptionServiceTests
    {
        // Mocks
        private readonly IUnitOfWork _unitOfWork;

        // Services
        private readonly ISubscriptionService _subscription;

        public SubscriptionServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _subscription = new SubscriptionService(_unitOfWork);
        }

        // CreateAsync
        [Fact]
        public async Task CreateAsync_WithNonExistingSubscriptionPlan_ThrowsNotFoundException()
        {
            // Arrange 
            _unitOfWork.SubscriptionPlans.GetByIdAsync(Arg.Any<int>()).Returns((SubscriptionPlan?)null);

            var dto = new CreateSubscriptionDto { MemberId = 1, SubscriptionPlanId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _subscription.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WithNonExistingMember_ThrowsNotFoundException()
        {
            // Arrange
            var subscriptionPlan = new SubscriptionPlan { Id = 1, Name = "Plan", Price = 25000, DurationDays = 30 };

            _unitOfWork.SubscriptionPlans.GetByIdAsync(1).Returns(subscriptionPlan);

            _unitOfWork.Members.GetByIdAsync(Arg.Any<int>()).Returns((Member?)null);

            var dto = new CreateSubscriptionDto { MemberId = 1, SubscriptionPlanId = 1 };

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _subscription.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WithNoActiveSubscription_CreatesNewSubscriptionWithoutTransaction()
        {
            // Arrange
            var subscriptionPlan = new SubscriptionPlan { Id = 1, Name = "Plan", Price = 25000, DurationDays = 30 };
            var member = new Member { Id = 1, FullName = "Mohammed Abed", Phone = "052256664" };

            _unitOfWork.SubscriptionPlans.GetByIdAsync(1).Returns(subscriptionPlan);
            _unitOfWork.Members.GetByIdAsync(1).Returns(member);

            _unitOfWork.Subscriptions.GetActiveByMemberIdAsync(Arg.Any<int>()).Returns((Subscription?)null);

            _unitOfWork.Subscriptions
                .When(s => s.Add(Arg.Any<Subscription>()))
                .Do(s => s.Arg<Subscription>().Id = 1);

            _unitOfWork.SaveChangesAsync().Returns(1);

            var dto = new CreateSubscriptionDto { MemberId = 1, SubscriptionPlanId = 1 };

            // Act
            var result = await _subscription.CreateAsync(dto);

            // Assert
            var expectedEndDate = DateTime.UtcNow.AddDays(subscriptionPlan.DurationDays);
            Assert.True((expectedEndDate - result.EndDate).Duration() < TimeSpan.FromSeconds(2));

            _unitOfWork.Subscriptions.Received(1).Add(Arg.Any<Subscription>());
            await _unitOfWork.DidNotReceive().BeginTransactionAsync();

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_WithActiveSubscription_MergesRemainingDaysAndUsesTransaction()
        {
            // Arrange
            var subscriptionPlan = new SubscriptionPlan { Id = 1, Name = "Plan", Price = 25000, DurationDays = 30 };
            var member = new Member { Id = 1, FullName = "Mohammed Abed", Phone = "052256664" };
            var activeSubscription = new Subscription
            { Id = 1, MemberId = 1, SubscriptionPlanId = 1, StartDate = DateTime.UtcNow.AddDays(-20), EndDate = DateTime.UtcNow.AddDays(10) };

            _unitOfWork.SubscriptionPlans.GetByIdAsync(1).Returns(subscriptionPlan);
            _unitOfWork.Members.GetByIdAsync(1).Returns(member);
            _unitOfWork.Subscriptions.GetActiveByMemberIdAsync(1).Returns(activeSubscription);

            _unitOfWork.Subscriptions
                .When(s => s.Add(Arg.Any<Subscription>()))
                .Do(s => s.Arg<Subscription>().Id = 2);

            _unitOfWork.SaveChangesAsync().Returns(1);

            var dto = new CreateSubscriptionDto { MemberId = 1, SubscriptionPlanId = 1 };

            // Act
            var result = await _subscription.CreateAsync(dto);

            // Assert
            Assert.True((DateTime.UtcNow - activeSubscription.EndDate).Duration() < TimeSpan.FromSeconds(2));

            var expectedEndDate = DateTime.UtcNow.AddDays(subscriptionPlan.DurationDays + 10);
            Assert.True((expectedEndDate - result.EndDate).Duration() < TimeSpan.FromSeconds(2));

            _unitOfWork.Subscriptions.Received(1).Add(Arg.Any<Subscription>());
            await _unitOfWork.Received(1).BeginTransactionAsync();
            await _unitOfWork.Received(1).SaveChangesAsync();
            await _unitOfWork.Received(1).CommitTransactionAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
        }
    }
}