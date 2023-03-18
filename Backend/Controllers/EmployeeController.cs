using FeeCollectorApplication.Models;
using FeeCollectorApplication.Models.DtoModel;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllEmployee()
        {
            var role = await _roleManager.FindByNameAsync(SD.Role_Employee);
            if (role == null)
            {
                return NotFound();
            }
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            return Ok(usersInRole);
        }
        [HttpGet("id")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<ActionResult<ApplicationUser>> GetEmployeeById(string id)
        {
            var role = await _roleManager.FindByNameAsync(SD.Role_Employee);
            if (role == null)
            {
                return NotFound();
            }
            var userInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            var employee = userInRole.FirstOrDefault(u => u.Id == id);
            return Ok(employee);
        }

        [HttpGet("employee/request")]
        public async Task<IActionResult> GetListEmployeesRequests()
        {
            var model = await _unitOfWork.EmployeeRequest.GetAllAsync();
            if (model == null)
            {
                return NotFound("No employee request found!");
            }
            return Ok(model);
        }

        [HttpPost("request")] //For Employee Request to Become a Employee of Store
        [AllowAnonymous]
        public async Task<IActionResult> RegisterEmployee([FromBody] EmployeeRegisterRequestDTO model)
        {
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Name.ToLower() == model.Name.ToLower());
            if (userFromDb != null)
            {
                return BadRequest("This user has already exist!!!");
            }
            if (model.citizenIdentification == null)
            {
                return BadRequest("This employee must have Citizen Identification");
            }

            var newEmployeeRequest = new EmployeeRequest()
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                AvatarLink = model.AvatarLink,
                Address = model.Address,
                citizenIdentification = model.citizenIdentification
            };
            await _unitOfWork.EmployeeRequest.Add(newEmployeeRequest);
            await _unitOfWork.Save();

            return Ok("Requested");
        }
    }
}
