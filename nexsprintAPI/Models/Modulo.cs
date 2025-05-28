using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using nexsprintAPI.Models;

namespace nexsprintAPI.Models
{
    public class Modulo
    {
        public int ModuloId { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string PDF_Url { get; set; }
        [Required]
        public string descricao { get; set; }
        [Required]
        public int TotalPaginas { get; set; }

        // Relação de muitos pra muitos
        [JsonIgnore]
        public ICollection<UserModulo> UsersModulos { get; set; }
    }
}