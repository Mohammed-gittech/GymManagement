
using GymManagement.BLL.DTOs.Payment;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Entities.Enums;
using GymManagement.DAL.UnitOfWork;
using NSubstitute;

namespace GymManagement.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;

        // Services
        private readonly IPaymentService _paymentService;

        public PaymentServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _paymentService = new PaymentService(_unitOfWork);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistingSubscription_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWork.Subscriptions.GetByIdAsync(Arg.Any<int>()).Returns((Subscription?)null);

            var dto = new CreatePaymentDto { SubscriptionId = 1, Amount = 25000, PaymentMethod = PaymentMethod.Cash };

            // Act + Assert 
            await Assert.ThrowsAsync<NotFoundException>(() => _paymentService.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WithExistingSubscription_CreatesPaymentSuccessfully()
        {
            // Arrange
            var subscription = new Subscription
            {
                Id = 1,
                MemberId = 1,
                SubscriptionPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _unitOfWork.Subscriptions.GetByIdAsync(1).Returns(subscription);

            _unitOfWork.Payments.When(p => p.Add(Arg.Any<Payment>())).Do(p => p.Arg<Payment>().Id = 1);

            _unitOfWork.SaveChangesAsync().Returns(1);

            var dto = new CreatePaymentDto { SubscriptionId = 1, Amount = 25000, PaymentMethod = PaymentMethod.Cash };

            // Act
            var result = await _paymentService.CreateAsync(dto);

            // Assert
            Assert.True((DateTime.UtcNow - result.PaymentDate).Duration() < TimeSpan.FromSeconds(2));

            _unitOfWork.Payments.Received(1).Add(Arg.Any<Payment>());
            await _unitOfWork.Received(1).SaveChangesAsync();

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(25000, result.Amount);
        }
    }
}