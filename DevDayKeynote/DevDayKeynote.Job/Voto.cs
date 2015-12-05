using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DevDayKeynote.Job
{
    public class Voto
    {
        public int Id { get; set; }

        public string Usuario { get; set; }

        public string Comunidad { get; set; }
    }
}