namespace stb_backend.DTOs
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Matricule { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}

