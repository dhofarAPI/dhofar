using System.ComponentModel.DataAnnotations;

namespace dhofarAPI.DTOS.User
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
