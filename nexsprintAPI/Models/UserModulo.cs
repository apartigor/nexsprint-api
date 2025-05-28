namespace nexsprintAPI.Models
{
    public class UserModulo
    {
        // FK
        public string NomeUsuario { get; set; }
        public User User { get; set; }

        // FK
        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; }

        public double Progresso { get; set; }


    }
}