namespace IOM.Models.ApiControllerModels
{
    public class ResetPasswordModel
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordModel
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}