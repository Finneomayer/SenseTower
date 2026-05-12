namespace API.Models.Registration
{
    public class ValidationError
    {
        public string Message { get; set; }
        public string Login { get; set; }
        public string InvalidUserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Space { get; set; }
    }
}