using LambdaAlexaLastDance2.Models;
using Microsoft.EntityFrameworkCore;

namespace LambdaAlexaLastDance2.Data
{
    public class CineContext : DbContext
    {
        public CineContext(DbContextOptions<CineContext> options) : base(options) { }

        public DbSet<Pelicula> Peliculas { get; set; }
    }
}
