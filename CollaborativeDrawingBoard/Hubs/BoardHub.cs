using CollaborativeDrawingBoard.Models;
using Microsoft.AspNetCore.SignalR;

namespace CollaborativeDrawingBoard.Hubs
{
    public class BoardHub : Hub
    {
        public async Task RefreshBoards(List<Board> boards) => await Clients.All.SendAsync("RefreshBoards", boards);

        public async Task RefreshBoard(string svg) => await Clients.All.SendAsync("RefreshBoard", svg);
    }
}
