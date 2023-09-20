using Microsoft.AspNetCore.SignalR;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Hubs
{
    public class ChatHub: Hub
    {
        private readonly MyBankContext _bankContext;
        private readonly UserRepository userRepo;

        public ChatHub()
        {
            _bankContext = new MyBankContext();
            userRepo = new UserRepository(_bankContext);
        }
        public async Task SendMessage(string message)
        {
            int userID = Convert.ToInt32(Context.GetHttpContext().Request.Cookies["user"]);
            User user = userRepo.GetUserById(userID);
            await Clients.All.SendAsync("ReceiveMessage", user.Name, message);
        }
    }
}
