namespace Tema2_Anton_Cosmin;

public interface IBookRepository
{
    Book? GetById(int id);
    (IEnumerable<Book> Items, int TotalCount) GetPaged(int page, int pageSize);
    IEnumerable<Book> GetAll();
    Book Create(string title, string author, int year);
    bool Update(int id, string title, string author, int year);
    bool Delete(int id);
}