using LiMS.Domain;

namespace LiMS.Application.Test
{
    public class LibraryServiceTests
    {
        private readonly Mock<IRepository<Book>> _mockBookRepository;
        private readonly Mock<IRepository<Member>> _mockMemberRepository;
        private readonly LibraryService _libraryService;

        public LibraryServiceTests()
        {
            _mockBookRepository = new Mock<IRepository<Book>>();
            _mockMemberRepository = new Mock<IRepository<Member>>();
            _libraryService = new LibraryService(_mockBookRepository.Object, _mockMemberRepository.Object);
        }

        [Fact]
        public void AddBook_ShouldCallAddOnRepository()
        {
            var book = new Book { BookId = 1, Title = "Test Book", Author = "Test Author" };
            _libraryService.AddBook(book);
            _mockBookRepository.Verify(repo => repo.Add(book), Times.Once);
        }

        [Fact]
        public void UpdateBook_ShouldCallUpdateOnRepository()
        {
            var book = new Book { BookId = 1, Title = "Test Book", Author = "Test Author" };
            _libraryService.UpdateBook(book);
            _mockBookRepository.Verify(repo => repo.Update(book), Times.Once);
        }

        [Fact]
        public void DeleteBook_ShouldCallDeleteOnRepository()
        {
            int bookId = 1;
            _libraryService.DeleteBook(bookId);
            _mockBookRepository.Verify(repo => repo.Delete(bookId), Times.Once);
        }

        [Fact]
        public void GetAllBooks_ShouldReturnListOfBooks()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Test Book 1", Author = "Author 1" },
                new Book { BookId = 2, Title = "Test Book 2", Author = "Author 2" }
            };
            _mockBookRepository.Setup(repo => repo.GetAll()).Returns(books);

            var result = _libraryService.GetAllBooks();

            Assert.Equal(2, result.Count);
            Assert.Equal("Test Book 1", result.First().Title);
        }

        [Fact]
        public void GetBookById_ShouldReturnBook()
        {
            var book = new Book { BookId = 1, Title = "Test Book", Author = "Test Author" };
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            var result = _libraryService.GetBookById(1);

            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public void AddMember_ShouldCallAddOnRepository()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@example.com" };
            _libraryService.AddMember(member);
            _mockMemberRepository.Verify(repo => repo.Add(member), Times.Once);
        }

        [Fact]
        public void UpdateMember_ShouldCallUpdateOnRepository()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@example.com" };
            _libraryService.UpdateMember(member);
            _mockMemberRepository.Verify(repo => repo.Update(member), Times.Once);
        }

        [Fact]
        public void DeleteMember_ShouldCallDeleteOnRepository()
        {
            int memberId = 1;
            _libraryService.DeleteMember(memberId);
            _mockMemberRepository.Verify(repo => repo.Delete(memberId), Times.Once);
        }

        [Fact]
        public void GetAllMembers_ShouldReturnListOfMembers()
        {
            var members = new List<Member>
            {
                new Member { MemberID = 1, Name = "Test Member 1", Email = "member1@example.com" },
                new Member { MemberID = 2, Name = "Test Member 2", Email = "member2@example.com" }
            };
            _mockMemberRepository.Setup(repo => repo.GetAll()).Returns(members);

            var result = _libraryService.GetAllMembers();

            Assert.Equal(2, result.Count);
            Assert.Equal("Test Member 1", result.First().Name);
        }

        [Fact]
        public void GetMemberById_ShouldReturnMember()
        {
            var member = new Member { MemberID = 1, Name = "Test Member", Email = "test@example.com" };
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            var result = _libraryService.GetMemberById(1);

            Assert.Equal("Test Member", result.Name);
        }

        [Fact]
        public void BorrowBook_ShouldUpdateBookStatusAndCallUpdateOnRepository()
        {
            var book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = false };
            var member = new Member { MemberID = 1, Name = "Test Member" };

            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);
            _mockMemberRepository.Setup(repo => repo.GetById(1)).Returns(member);

            _libraryService.BorrowBook(1, 1);

            Assert.True(book.IsBorrowed);
            _mockBookRepository.Verify(repo => repo.Update(book), Times.Once);
        }

        [Fact]
        public void ReturnBook_ShouldUpdateBookStatusAndCallUpdateOnRepository()
        {
            var book = new Book { BookId = 1, Title = "Test Book", IsBorrowed = true };
            _mockBookRepository.Setup(repo => repo.GetById(1)).Returns(book);

            _libraryService.ReturnBook(1);

            Assert.False(book.IsBorrowed);
            _mockBookRepository.Verify(repo => repo.Update(book), Times.Once);
        }

        [Fact]
        public void GetAllBorrowedBooks_ShouldReturnListOfBorrowedBooks()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Test Book 1", IsBorrowed = true },
                new Book { BookId = 2, Title = "Test Book 2", IsBorrowed = false }
            };
            _mockBookRepository.Setup(repo => repo.GetAll()).Returns(books);

            var result = _libraryService.GetAllBorrowedBooks();

            Assert.Single(result);
            Assert.Equal("Test Book 1", result.First().Title);
        }
    }
}
