namespace Tema2_Anton_Cosmin;

public class CommandsQueries
{
    public record CreateBookCommand(string Title, string Author, int Year);
    public record UpdateBookCommand(int Id, string Title, string Author, int Year);
    public record DeleteBookCommand(int Id);
    public record GetBookByIdQuery(int Id);
    public record GetBooksQuery(int Page, int PageSize);
}