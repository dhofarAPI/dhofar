using dhofarAPI.DTOS.User;
using dhofarAPI.InterFaces;
using dhofarAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dhofarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIDentityUser _context;

        public AuthController(IIDentityUser context)
        {
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var user = await _context.Register(registerDTO, this.ModelState, User);
            if (ModelState.IsValid)
            {
                if (user != null)
                    return user;

                else
                    return NotFound();
            }
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Login(loginDTO.UserName, loginDTO.Password);

            if (ModelState.IsValid)
            {
                if (user != null)
                    return user;

                else
                    return Unauthorized();
            }
            return BadRequest(new ValidationProblemDetails(ModelState));
        }
    }
}
