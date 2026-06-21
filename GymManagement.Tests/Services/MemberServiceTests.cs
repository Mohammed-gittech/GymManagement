using System.Threading.Tasks;
using GymManagement.BLL.DTOs.Members;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Helpers;
using GymManagement.DAL.UnitOfWork;
using NSubstitute;

namespace GymManagement.Tests.Services
{

    public class MemberServiceTests
    {
        // Mocks
        private readonly IUnitOfWork _unitOfWork;

        // Services 
        private readonly IMemberService _memberService;

        public MemberServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _memberService = new MemberService(_unitOfWork);
        }

        // Get All Members 
        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedMembers()
        {
            // Arrange
            var members = new List<Member>
            {
                new Member{Id = 1,FullName = "Member1", Phone = "0501111111"},
                new Member{Id = 2,FullName = "Member2", Phone = "0502222222"},
                new Member{Id = 3,FullName = "Member3", Phone = "0503333333"},
            };

            var pagedResponse = new PagedResponse<Member>
            {
                Items = members,
                TotalCount = 3,
                PageNumber = 1,
                PageSize = 10
            };

            _unitOfWork.Members
                .GetPagedAsync(Arg.Any<PaginationParams>())
                .Returns(pagedResponse);

            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _memberService.GetAllAsync(paginationParams);

            // Assert
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(3, result.TotalCount);
        }

        // Get Member By Id 
        [Fact]
        public async Task GetByIdAsync_WithExistingMember_ReturnsMemberResponseDto()
        {
            // Arrange
            var member = new Member { Id = 1, FullName = "Member", Phone = "055111111" };

            _unitOfWork.Members
                .GetByIdAsync(1)
                .Returns(member);

            // Act 
            var result = await _memberService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.NotEmpty(result.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingMember_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWork.Members
                .GetByIdAsync(Arg.Any<int>())
                .Returns((Member?)null);

            // Act + Assert

            await Assert.ThrowsAsync<NotFoundException>(() => _memberService.GetByIdAsync(1));
        }

        // Create Member
        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsMemberResponseDto()
        {
            // Arrange 
            _unitOfWork.Members.GetByPhoneAsync(Arg.Any<string>()).Returns((Member?)null);

            _unitOfWork.Members.GetByEmailAsync(Arg.Any<string>()).Returns((Member?)null);

            _unitOfWork.Members.When(m => m.Add(Arg.Any<Member>())).Do(m => m.Arg<Member>().Id = 1);

            _unitOfWork.SaveChangesAsync().Returns(1);

            var dto = new CreateMemberDto { FullName = "Member", Phone = "055011111", Email = "a@a.com" };

            // Act 
            var result = await _memberService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Member", result.FullName);
            Assert.Equal("055011111", result.Phone);
            Assert.Equal("a@a.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_WithExistingPhone_ThrowsValidationException()
        {
            // Arrange
            var member = new Member { Id = 1, FullName = "Member", Phone = "05050000" };

            _unitOfWork.Members.GetByPhoneAsync("05050000").Returns(member);

            var dto = new CreateMemberDto { FullName = "Member", Phone = "05050000" };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _memberService.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WithExistingEmail_ThrowsValidationException()
        {
            // Arrange 
            var member = new Member { Id = 1, FullName = "Member", Phone = "05050000", Email = "a@a.com" };

            _unitOfWork.Members.GetByPhoneAsync(Arg.Any<string>()).Returns((Member?)null);

            _unitOfWork.Members.GetByEmailAsync("a@a.com").Returns(member);

            var dto = new CreateMemberDto { FullName = "Member", Phone = "05050000", Email = "a@a.com" };
            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _memberService.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_WithoutEmail_CreatesMemberSuccessfully()
        {
            // Arrange 
            _unitOfWork.Members.GetByPhoneAsync(Arg.Any<string>()).Returns((Member?)null);

            _unitOfWork.Members.When(m => m.Add(Arg.Any<Member>())).Do(m => m.Arg<Member>().Id = 1);

            _unitOfWork.SaveChangesAsync().Returns(1);

            var dto = new CreateMemberDto { FullName = "Member", Phone = "055011111" };

            // Act 
            var result = await _memberService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Member", result.FullName);
            Assert.Equal("055011111", result.Phone);

            // Verify GetByEmailAsync was never called since email was not provided
            await _unitOfWork.Members.DidNotReceive().GetByEmailAsync(Arg.Any<string>());
        }

        // Upadate Member
        [Fact]
        public async Task UpdateAsync_WithValidData_ReturnsMemberResponseDto()
        {
            // Arrange
            var member = new Member { Id = 1, FullName = "Member", Phone = "05500000", Email = "a@a.com" };

            _unitOfWork.Members.GetByIdTrackedAsync(1).Returns(member);

            _unitOfWork.Members.GetByPhoneAsync(Arg.Any<string>()).Returns((Member?)null);

            _unitOfWork.Members.GetByEmailAsync(Arg.Any<string>()).Returns((Member?)null);

            var dto = new UpdateMemberDto { FullName = "Member1", Phone = "05500000111", Email = "a@aa.com" };

            _unitOfWork.SaveChangesAsync().Returns(1);

            // Act
            var result = await _memberService.UpdateAsync(dto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Member1", result.FullName);
            Assert.Equal("05500000111", result.Phone);
            Assert.Equal("a@aa.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingMember_ThrowsNotFoundException()
        {
            // Arrange 
            _unitOfWork.Members.GetByIdTrackedAsync(Arg.Any<int>()).Returns((Member?)null);

            var dto = new UpdateMemberDto { FullName = "Member1", Phone = "05500000111", Email = "a@aa.com" };

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _memberService.UpdateAsync(dto, 1));
        }

        [Fact]
        public async Task UpdateAsync_WithPhoneBelongingToAnotherMember_ThrowsValidationException()
        {
            // Arrange
            var member1 = new Member { Id = 1, FullName = "Member1", Phone = "05500000", Email = "a@a.com" };
            var member2 = new Member { Id = 2, FullName = "Member2", Phone = "05111110", Email = "b@b.com" };

            _unitOfWork.Members.GetByIdTrackedAsync(1).Returns(member1);

            _unitOfWork.Members.GetByPhoneAsync("05111110").Returns(member2);

            var dto = new UpdateMemberDto { FullName = "Member2", Phone = "05111110", Email = "b@b.com" };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _memberService.UpdateAsync(dto, 1));
        }

        [Fact]
        public async Task UpdateAsync_WithEmailBelongingToAnotherMember_ThrowsValidationException()
        {
            // Arrange
            var member1 = new Member { Id = 1, FullName = "Member1", Phone = "05500000", Email = "a@a.com" };
            var member2 = new Member { Id = 2, FullName = "Member2", Phone = "05111110", Email = "b@b.com" };

            _unitOfWork.Members.GetByIdTrackedAsync(1).Returns(member1);

            _unitOfWork.Members.GetByPhoneAsync(Arg.Any<string>()).Returns((Member?)null);

            _unitOfWork.Members.GetByEmailAsync("b@b.com").Returns(member2);

            var dto = new UpdateMemberDto { FullName = "Member2", Phone = "05111110", Email = "b@b.com" };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _memberService.UpdateAsync(dto, 1));
        }

        // Soft Delete Member
        [Fact]
        public async Task DeleteAsync_WithExistingMember_DeletesSuccessfully()
        {
            // Arrange
            var member = new Member { Id = 1, FullName = "Member1", Phone = "05500000", Email = "a@a.com" };

            _unitOfWork.Members.GetByIdTrackedAsync(1).Returns(member);

            _unitOfWork.SaveChangesAsync().Returns(1);


            // Act
            await _memberService.DeleteAsync(1);

            // Assert
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingMember_ThrowsNotFoundException()
        {
            // Arrange 
            _unitOfWork.Members.GetByIdTrackedAsync(Arg.Any<int>()).Returns((Member?)null);

            // Act + Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _memberService.DeleteAsync(1));
        }
    }
}