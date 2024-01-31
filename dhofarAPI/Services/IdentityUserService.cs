using dhofarAPI.DTOS.User;
using dhofarAPI.InterFaces;
using dhofarAPI.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Win32;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Security.Claims;

namespace dhofarAPI.Services
{
    public class IdentityUserService : IIDentityUser
    {
        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        private readonly JWTTokenService _jWTTokenService;

        private readonly IConfiguration _configuration;


        public IdentityUserService(SignInManager<User> signInManager, UserManager<User> userManager , JWTTokenService jWTTokenService, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jWTTokenService = jWTTokenService;
            _configuration = configuration;
        }


        public async Task<UserDTO> Register(RegisterDTO model, ModelStateDictionary modelState, ClaimsPrincipal principal)
        {
            var user = new User()
            {
                UserName = model.UserName,
                FullName = model.Fullname,
                Email = model.Email,
                CodeNumber = model.CodeNumber,
                PhoneNumber = model.PhoneNumber

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                IList<string> role = new List<string>() { "User" };
                await _userManager.AddToRolesAsync(user, role);
                return new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Token = await _jWTTokenService.GetToken(user, System.TimeSpan.FromHours(2)),
                    Roles = await _userManager.GetRolesAsync(user),
                };
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    var errorMessage =  error.Code.Contains("Email") ? nameof(model.Email) :
                                       error.Code.Contains("UserName") ? nameof(model.UserName) :
                                       error.Code.Contains("Fullname") ? nameof(model.Fullname) : 
                                         error.Code.Contains("CodeNumber") ? nameof(model.CodeNumber) :
                                         error.Code.Contains("PhoneNumber") ? nameof(model.PhoneNumber) :
                                         error.Code.Contains("Password") ? nameof(model.Password) :
                                         error.Code.Contains("ConfirmPassword") ? nameof(model.ConfirmPassword) :
                                       "";
                    modelState.AddModelError(errorMessage, error.Description);

                };
                return null;
            }
        }

        public async Task<UserDTO> Login(string username,string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                bool isValidPass = await _userManager.CheckPasswordAsync(user, password);
                if (isValidPass)
                {
                    return new UserDTO
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Token = await _jWTTokenService.GetToken(user, System.TimeSpan.FromHours(2)),
                        Roles = await _userManager.GetRolesAsync(user)
                    };
                }
            }
            return null;
        }

        public async Task Logout(ClaimsPrincipal principal)
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<bool> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await SendEmailAsync(email , "your Token" ,token);
                return true;
            }
            return false; 
        }


        private async Task SendEmailAsync(string recipientEmail, string subject, string htmlContent)
        {
            string sendGridKey  = _configuration["SendGrid:SendGridKey"];

            var client = new SendGridClient(sendGridKey);
            var from = new EmailAddress("your-email@example.com", "Your Name");
            var to = new EmailAddress(recipientEmail);
            var body = $"Your token is: {htmlContent}";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body); // Pass null as plain text body
            msg.HtmlContent = htmlContent; // Set HTML content

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            }
        }

        public async Task<(bool Success, IList<string> Errors)> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    return (true, null); 
                }
                else
                {
                    IList<string> errors = new List<string>();
                    foreach (var error in result.Errors)
                    {
                        
                        errors.Add(error.Description);
                    }
                    return (false, errors); 
                }
            }
            return (false, new List<string> { "User not found" }); 
        }

    }
}
