namespace Tema2_Anton_Cosmin;

public sealed class InMemoryBookRepository : IBookRepository
{
    private readonly ConcurrentDictionary<int, Book> _data = new();
    private int _nextId;

    public InMemoryBookRepository()
    {
        _nextId = 0;
    }

    public Book? GetById(int id) => _data.TryGetValue(id, out var b) ? b : null;

    public (IEnumerable<Book> Items, int TotalCount) GetPaged(int page, int pageSize)
    {
        var all = _data.Values.OrderBy(b => b.Id).ToArray();
        var total = all.Length;
        var items = all.Skip((page - 1) * pageSize).Take(pageSize);
        return (items, total);
    }

    public IEnumerable<Book> GetAll() => _data.Values.OrderBy(b => b.Id);

    public Book Create(string title, string author, int year)
    {
        var id = Interlocked.Increment(ref _nextId);
        var book = new Book(id, title, author, year);
        _data[id] = book;
        return book;
    }

    public bool Update(int id, string title, string author, int year)
    {
        if (!_data.TryGetValue(id, out var existing)) return false;
        var updated = existing with { Title = title, Author = author, Year = year };
        _data[id] = updated;
        return true;
    }

    public bool Delete(int id) => _data.TryRemove(id, out _);
}