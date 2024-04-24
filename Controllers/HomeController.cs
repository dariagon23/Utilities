
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utilities.Database;
using Utilities.Database.Models;
using Utilities.Models;
using Utilities.MVC.Models.Tarrifs;

namespace Utilities.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        protected readonly UtilitiesContext _context;


        public HomeController(ILogger<HomeController> logger, UtilitiesContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async  Task<IActionResult> Index()
        {
            var utilities = await _context.Set<Utility>().AsQueryable().Include(ut => ut.Tariffs).ToListAsync();
            return View(utilities);
        }

        public async Task<IActionResult> Calculate(int utilId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userRegion = await _context.Regions.FirstOrDefaultAsync(r => r.Users.Any(r => r.Id == userId));

            var calcModel = new CalculateModel()
            {
                RegionId = userRegion is not null ? userRegion.RegionId : 0,
                Regions = await _context.Regions.Select(re => new SelectListItem()
                {
                    Value = re.RegionId.ToString(),
                    Text = re.RegionName
                }) .ToListAsync()
            };

            if (userRegion is not null)
            {
                calcModel.Regions.Where(r => r.Text == userRegion.RegionName).First().Selected = true;
            }

            return View(calcModel); 
        }

        //TODO:
        // add valid for decimal
        public async Task<IActionResult> CalculateForTarrifs(CalculateModel model) 
        {
            var tariffs = await _context.Tariffs.Where(t => t.Provider.RegionId == model.RegionId)
                .ToListAsync();
            return View("Cal", tariffs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
