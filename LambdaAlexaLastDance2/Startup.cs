using LambdaAlexaLastDance2.Data;
using LambdaAlexaLastDance2.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LambdaAlexaLastDance2
{
    public class Startup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            //RECUPERAMOS EL FICHERO DE CONFIGURACION
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            //RECUPERAMOS LA CADENA DE CONEXION
            string connectionString = configuration.GetConnectionString("MySql");
            services.AddDbContext<CineContext>
                (options => options.UseMySQL(connectionString));
            services.AddTransient<RepositoryPeliculas>();
            return services.BuildServiceProvider();
        }
    }
}
