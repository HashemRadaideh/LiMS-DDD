using Newtonsoft.Json;

using LiMS.Domain;

namespace LiMS.Infrastructure.Test
{
    public class BookRepositoryTests
    {
        private readonly string _booksFile = "TestBooks.json";

        private readonly BookRepository _repository;

        public BookRepositoryTests()
        {
            _repository = new BookRepository(_booksFile);
        }

        [Fact]
        public void Add_ShouldAddBookToRepository()
        {
            var book = new Book { BookId = 1, Title = "Test Book" };
            _repository.Add(book);
            var books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(_booksFile));
            Assert.Contains(books, b => b.BookId == book.BookId);
        }

        [Fact]
        public void GetAll_ShouldReturnBooks()
        {
            var books = _repository.GetAll();
            Assert.NotEmpty(books);
        }

        [Fact]
        public void GetById_ShouldReturnBook()
        {
            var book = _repository.GetById(1);
            Assert.NotNull(book);
        }

        [Fact]
        public void Delete_ShouldRemoveBook()
        {
            _repository.Delete(1);
            var book = _repository.GetById(1);
            Assert.Null(book);
        }
    }
}
