namespace DevDayKeynote.Job
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class VotoDbContext : DbContext
    {
        public VotoDbContext()
            : base("name=AzureWebJobsDatabase")
        {
        }

        public virtual DbSet<Voto> Votos { get; set; }
    }
}
