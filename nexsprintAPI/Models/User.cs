using System.ComponentModel.DataAnnotations;

namespace nexsprintAPI.Models
{
    public class User
    {
        [Key]
        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        // Relação de muitos pra muitos
        public ICollection<UserModulo> UsersModulos { get; set; }
    }
}