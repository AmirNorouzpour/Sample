namespace API.Models
{
    public class UserSelectDto : BaseDto<int>
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }

    }

    public class UserDto : UserSelectDto
    {
        public string Password { get; set; }
        public string RePassword { get; set; }

    }
}
