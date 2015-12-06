using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace DevDayKeynote.Job
{
    [Table("Votos")]
    public class Voto
    {
        public int Id { get; set; }

        public string Usuario { get; set; }

        public string Comunidad { get; set; }
    }
}