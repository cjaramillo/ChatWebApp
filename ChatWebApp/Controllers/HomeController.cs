using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatWebApp.Models;
using ChatWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPCComunicator;
using ChatWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private int _topOnSelect;
        private RpcClient _rpcClient;
        private IHubContext<ChatHub> _hubContext { get; set; }

        public HomeController(ApplicationDbContext context, UserManager<AppUser> userManager, IConfiguration configuration, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _topOnSelect = 50;
            int.TryParse(_configuration["TopOnSelect"] ?? "50", out _topOnSelect);
            _rpcClient = new RpcClient(configuration);
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var messages = await this._context.Messages.OrderByDescending(u => u.When).Take(_topOnSelect).ToListAsync();
            return View(messages);
        }

        public async Task<IActionResult> Create(Message message)
        {
            if (ModelState.IsValid)
            {
                message.UserName = User.Identity.Name;
                var sender = await _userManager.GetUserAsync(User);
                message.UserID = sender.Id;
                await _context.Messages.AddAsync(message);
                if (message.Text.StartsWith("/stock="))
                {
                    // Fire and forget call to not block
                    _ = Task.Run(() => callQueue(message));
                }
                else
                {
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
            return Error();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public void callQueue(Message message)
        {
            var response = _rpcClient.Call(message.Text);
            Message m1 = new Message();
            m1.UserName = "StockBot";
            m1.Text = response;
            _hubContext.Clients.All.SendAsync("receiveMessage", m1);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
