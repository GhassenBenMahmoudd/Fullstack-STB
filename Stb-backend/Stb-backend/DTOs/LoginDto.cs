namespace stb_backend.DTOs
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Matricule { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public class UserInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Matricule { get; set; }
        public string Role { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public object Permissions { get; set; } = new();
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserInfoDto User { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}

