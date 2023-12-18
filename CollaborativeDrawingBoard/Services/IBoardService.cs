using CollaborativeDrawingBoard.Models;

namespace CollaborativeDrawingBoard.Services
{
    public interface IBoardService
    {
        Task<int> CreateBoardAsync(Board board);

        Task<List<Board>> GetReverseBoardsAsync();

        Task<string> GetSvgContentAsync(int id);

        Task UpdateBoard(int id, string svg);
    }
}
