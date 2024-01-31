using dhofarAPI.DTOS.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace dhofarAPI.InterFaces
{
    public interface IIDentityUser
    {
        Task<UserDTO> Register([FromForm] RegisterDTO model, ModelStateDictionary modelState, ClaimsPrincipal principal);

        Task<UserDTO> Login([FromForm] string username, [FromForm] string password);

        Task Logout(ClaimsPrincipal principal);
    }
}
