using LambdaAlexaLastDance2.Data;
using LambdaAlexaLastDance2.Models;
using Microsoft.EntityFrameworkCore;

namespace LambdaAlexaLastDance2.Repositories
{
    public class RepositoryPeliculas
    {
        private CineContext context;
        public RepositoryPeliculas(CineContext context)
        {
            this.context = context;
        }

        public async Task<List<Pelicula>> GetPeliculasAsync()
        {
            return await this.context.Peliculas.ToListAsync();
        }

        public async Task<Pelicula> FindPeliculaAsync(int id)
        {
            return await this.context.Peliculas.FirstOrDefaultAsync(x => x.IdPelicula == id);
        }
    }
}
