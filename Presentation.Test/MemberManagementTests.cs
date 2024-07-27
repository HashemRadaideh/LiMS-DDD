using LiMS.Application;
using LiMS.Domain;

namespace LiMS.Presentation.Test
{
    public class MemberManagementTests
    {
        private readonly Mock<LibraryService> _mockLibraryService;

        public MemberManagementTests()
        {
            _mockLibraryService = new Mock<LibraryService>(Mock.Of<IRepository<Book>>(), Mock.Of<IRepository<Member>>());
        }

        [Fact]
        public void AddNewMember_ValidInput_ShouldAddMember()
        {
            // Arrange
            var memberList = new List<Member>();
            _mockLibraryService.Setup(service => service.GetAllMembers()).Returns(memberList);
            _mockLibraryService.Setup(service => service.AddMember(It.IsAny<Member>())).Callback<Member>(member => memberList.Add(member));

            // Act
            var input = new StringReader("Sample Name\nsample@email.com\n");
            Console.SetIn(input);

            MemberManagement.AddNewMember(_mockLibraryService.Object);

            // Assert
            Assert.Single(memberList);
            var addedMember = memberList.First();
            Assert.Equal("Sample Name", addedMember.Name);
            Assert.Equal("sample@email.com", addedMember.Email);
        }

        [Fact]
        public void UpdateMember_ValidInput_ShouldUpdateMember()
        {
            // Arrange
            var member = new Member { MemberID = 1, Name = "Old Name", Email = "old@email.com" };
            var memberList = new List<Member> { member };
            _mockLibraryService.Setup(service => service.GetMemberById(1)).Returns(member);
            _mockLibraryService.Setup(service => service.UpdateMember(It.IsAny<Member>())).Callback<Member>(updatedMember =>
            {
                var index = memberList.FindIndex(m => m.MemberID == updatedMember.MemberID);
                if (index != -1) memberList[index] = updatedMember;
            });

            // Act
            var input = new StringReader("1\nNew Name\nnew@email.com\n");
            Console.SetIn(input);

            MemberManagement.UpdateMember(_mockLibraryService.Object);

            // Assert
            var updatedMember = memberList.First();
            Assert.Equal("New Name", updatedMember.Name);
            Assert.Equal("new@email.com", updatedMember.Email);
        }

        [Fact]
        public void DeleteMember_ValidInput_ShouldRemoveMember()
        {
            // Arrange
            var member = new Member { MemberID = 1, Name = "Sample Member" };
            var memberList = new List<Member> { member };
            _mockLibraryService.Setup(service => service.GetMemberById(1)).Returns(member);
            _mockLibraryService.Setup(service => service.DeleteMember(1)).Callback(() => memberList.Remove(member));

            // Act
            var input = new StringReader("1\n");
            Console.SetIn(input);

            MemberManagement.DeleteMember(_mockLibraryService.Object);

            // Assert
            Assert.Empty(memberList);
        }

        [Fact]
        public void ViewAllMembers_ShouldDisplayMembers()
        {
            // Arrange
            var members = new List<Member>
        {
            new Member { MemberID = 1, Name = "Member 1", Email = "email1@example.com" },
            new Member { MemberID = 2, Name = "Member 2", Email = "email2@example.com" }
        };
            _mockLibraryService.Setup(service => service.GetAllMembers()).Returns(members);

            // Act
            using var output = new StringWriter();
            Console.SetOut(output);

            MemberManagement.ViewAllMembers(_mockLibraryService.Object);

            // Assert
            var result = output.ToString();
            Assert.Contains("Member 1", result);
            Assert.Contains("Member 2", result);
        }
    }
}
