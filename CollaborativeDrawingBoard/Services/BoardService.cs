using CollaborativeDrawingBoard.Data;
using CollaborativeDrawingBoard.Hubs;
using CollaborativeDrawingBoard.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace CollaborativeDrawingBoard.Services
{
    public class BoardService : IBoardService
    {
        private readonly IHubContext<BoardHub> _hubContext;

        private readonly SqlTableDependency<Board> _tableDependency;

        private readonly object _lockObject = new();

        private readonly List<int> _changedBoardIds = [];

        public BoardService(IConfiguration configuration, IHubContext<BoardHub> hubContext)
        {
            _hubContext = hubContext;
            _tableDependency = new SqlTableDependency<Board>(configuration.GetConnectionString("DefaultConnection"), "Boards");
            _tableDependency.OnChanged += Changed;
            _tableDependency.Start();
        }

        public async Task<int> CreateBoardAsync(Board board)
        {
            using var context = new ApplicationDbContext();
            context.Boards.Add(board);
            await context.SaveChangesAsync();
            return board.Id;
        }

        public async Task<List<Board>> GetReverseBoardsAsync()
        {
            using var context = new ApplicationDbContext();
            var boards = await context.Boards.AsNoTracking().ToListAsync();
            boards.Reverse();
            return boards;
        }

        public async Task<string> GetSvgContentAsync(int id)
        {
            using var context = new ApplicationDbContext();
            var content = await context.Boards.AsNoTracking().FirstAsync(x => x.Id == id);
            return content.Svg;
        }

        public async Task UpdateBoard(int id, string svg)
        {
            using var context = new ApplicationDbContext();
            var board = await context.Boards.FirstAsync(x => x.Id == id);
            board.Svg = svg;
            await context.SaveChangesAsync();
        }

        private async Task SendBatchedUpdatesAsync()
        {
            List<int> changedIds;
            lock (_lockObject)
            {
                changedIds = new List<int>(_changedBoardIds);
                _changedBoardIds.Clear();
            }
            foreach (var id in changedIds)
            {
                var svg = await GetSvgContentAsync(id);
                await _hubContext.Clients.All.SendAsync("RefreshBoard", svg);
            }
            var boards = await GetReverseBoardsAsync();
            await _hubContext.Clients.All.SendAsync("RefreshBoards", boards);
        }

        private async void Changed(object sender, RecordChangedEventArgs<Board> e)
        {
            lock (_lockObject) _changedBoardIds.Add(e.Entity.Id);
            await SendBatchedUpdatesAsync();
        }
    }
}
