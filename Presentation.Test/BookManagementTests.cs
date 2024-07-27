using LiMS.Application;
using LiMS.Domain;

namespace LiMS.Presentation.Test
{
    public class BookManagementTests
    {
        private readonly Mock<LibraryService> _mockLibraryService;

        public BookManagementTests()
        {
            _mockLibraryService = new Mock<LibraryService>();
        }

        [Fact]
        public void AddNewBook_ValidInput_ShouldAddBook()
        {
            // Arrange
            var bookList = new List<Book>();
            _mockLibraryService.Setup(service => service.GetAllBooks()).Returns(bookList);
            _mockLibraryService.Setup(service => service.AddBook(It.IsAny<Book>())).Callback<Book>(book => bookList.Add(book));

            // Act
            var input = new StringReader("Sample Title\nSample Author\n");
            Console.SetIn(input);

            BookManagement.AddNewBook(_mockLibraryService.Object);

            // Assert
            Assert.Single(bookList);
            var addedBook = bookList.First();
            Assert.Equal("Sample Title", addedBook.Title);
            Assert.Equal("Sample Author", addedBook.Author);
        }

        [Fact]
        public void UpdateBook_ValidInput_ShouldUpdateBook()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Old Title", Author = "Old Author" };
            var bookList = new List<Book> { book };
            _mockLibraryService.Setup(service => service.GetBookById(1)).Returns(book);
            _mockLibraryService.Setup(service => service.UpdateBook(It.IsAny<Book>())).Callback<Book>(updatedBook =>
            {
                var index = bookList.FindIndex(b => b.BookId == updatedBook.BookId);
                if (index != -1) bookList[index] = updatedBook;
            });

            // Act
            var input = new StringReader("1\nNew Title\nNew Author\n");
            Console.SetIn(input);

            BookManagement.UpdateBook(_mockLibraryService.Object);

            // Assert
            var updatedBook = bookList.First();
            Assert.Equal("New Title", updatedBook.Title);
            Assert.Equal("New Author", updatedBook.Author);
        }

        [Fact]
        public void DeleteBook_ValidInput_ShouldRemoveBook()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Sample Title" };
            var bookList = new List<Book> { book };
            _mockLibraryService.Setup(service => service.GetBookById(1)).Returns(book);
            _mockLibraryService.Setup(service => service.DeleteBook(1)).Callback(() => bookList.Remove(book));

            // Act
            var input = new StringReader("1\n");
            Console.SetIn(input);

            BookManagement.DeleteBook(_mockLibraryService.Object);

            // Assert
            Assert.Empty(bookList);
        }

        [Fact]
        public void ViewAllBooks_ShouldDisplayBooks()
        {
            // Arrange
            var books = new List<Book>
        {
            new Book { BookId = 1, Title = "Book 1", Author = "Author 1" },
            new Book { BookId = 2, Title = "Book 2", Author = "Author 2" }
        };
            _mockLibraryService.Setup(service => service.GetAllBooks()).Returns(books);

            // Act
            using var output = new StringWriter();
            Console.SetOut(output);

            BookManagement.ViewAllBooks(_mockLibraryService.Object);

            // Assert
            var result = output.ToString();
            Assert.Contains("Book 1", result);
            Assert.Contains("Book 2", result);
        }
    }
}
