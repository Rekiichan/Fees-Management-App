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
    [Authorize(Roles = SD.Role_Employee + "," + SD.Role_Admin)]
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
        [Authorize(Roles = SD.Role_Admin)]
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

        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet("get-list-employee-requests")]
        public async Task<IActionResult> GetListEmployeesRequests()
        {
            var model = await _unitOfWork.EmployeeRequest.GetAllAsync();
            if (model == null)
            {
                return NotFound("no employee request found!");
            }
            return Ok(model);
        }

        [HttpPost("request")] //For Employee Request to Become a Employee of Store
        [AllowAnonymous]
        public async Task<IActionResult> RequestToRegisterEmployee([FromBody] EmployeeRegisterRequestDTO model)
        {
            var userFromDb = await _unitOfWork.ApplicationUser.GetAllAsync();
            var checkIsExist = userFromDb.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());
            if (checkIsExist != null)
            {
                return BadRequest("this user has already exist!!!");
            }
            if (model.CitizenIdentification == null)
            {
                return BadRequest("this employee must have citizen identification");
            }
            else if (model.CitizenIdentification.Length < 9)
            {
                return BadRequest("please enter the citizen identification again");
            }
            if (model.Email == null)
            {
                return BadRequest("this employee must have email address");
            }
            if (model.PhoneNumber == null)
            {
                return BadRequest("this employee must have phone number");
            }

            var newEmployeeRequest = new EmployeeRequest()
            {
                UserName = model.Email.ToLower().Split('@').First(),
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CitizenIdentification = model.CitizenIdentification
            };
 
            await _unitOfWork.EmployeeRequest.Add(newEmployeeRequest);
            await _unitOfWork.Save();

            return Ok("requested");
        }
    }
}

