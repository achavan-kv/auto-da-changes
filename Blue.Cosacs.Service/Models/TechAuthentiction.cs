namespace Blue.Cosacs.Service.Models
{
    public class TechAuthentiction
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public bool IsLocked { get; set; }

        public bool PwdChange { get; set; }

        public bool IsAuthorised { get; set; }
    }
}
