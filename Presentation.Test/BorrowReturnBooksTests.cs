using LiMS.Application;
using LiMS.Domain;

namespace LiMS.Presentation.Test
{
    public class BorrowReturnBooksTests
    {
        private readonly Mock<LibraryService> _mockLibraryService;

        public BorrowReturnBooksTests()
        {
            _mockLibraryService = new Mock<LibraryService>(Mock.Of<IRepository<Book>>(), Mock.Of<IRepository<Member>>());
        }

        [Fact]
        public void BorrowBook_ValidInput_ShouldBorrowBook()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Sample Book", IsBorrowed = false };
            var member = new Member { MemberID = 1, Name = "Sample Member" };
            _mockLibraryService.Setup(service => service.GetBookById(1)).Returns(book);
            _mockLibraryService.Setup(service => service.GetMemberById(1)).Returns(member);
            _mockLibraryService.Setup(service => service.UpdateBook(It.IsAny<Book>())).Callback<Book>(b =>
            {
                book.IsBorrowed = b.IsBorrowed;
                book.BorrowedDate = b.BorrowedDate;
                book.BorrowedBy = b.BorrowedBy;
            });

            // Act
            var input = new StringReader("1\n1\n");
            Console.SetIn(input);

            BorrowReturnBooks.BorrowBook(_mockLibraryService.Object);

            // Assert
            Assert.True(book.IsBorrowed);
            Assert.Equal(1, book.BorrowedBy);
        }

        [Fact]
        public void ReturnBook_ValidInput_ShouldReturnBook()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Sample Book", IsBorrowed = true };
            _mockLibraryService.Setup(service => service.GetBookById(1)).Returns(book);
            _mockLibraryService.Setup(service => service.UpdateBook(It.IsAny<Book>())).Callback<Book>(b =>
            {
                book.IsBorrowed = b.IsBorrowed;
                book.BorrowedDate = b.BorrowedDate;
                book.BorrowedBy = b.BorrowedBy;
            });

            // Act
            var input = new StringReader("1\n");
            Console.SetIn(input);

            BorrowReturnBooks.ReturnBook(_mockLibraryService.Object);

            // Assert
            Assert.False(book.IsBorrowed);
            Assert.Null(book.BorrowedDate);
            Assert.Null(book.BorrowedBy);
        }

        [Fact]
        public void ViewAllBorrowedBooks_ShouldDisplayBorrowedBooks()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Borrowed Book 1", IsBorrowed = true },
            new Book { BookId = 2, Title = "Available Book", IsBorrowed = false }
        };
            _mockLibraryService.Setup(service => service.GetAllBooks()).Returns(books);

            // Act
            using var output = new StringWriter();
            Console.SetOut(output);

            BorrowReturnBooks.ViewAllBorrowedBooks(_mockLibraryService.Object);

            // Assert
            var result = output.ToString();
            Assert.Contains("Borrowed Book 1", result);
            Assert.DoesNotContain("Available Book", result);
        }
    }
}
