using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using LambdaAlexaLastDance2.Data;
using LambdaAlexaLastDance2.Models;
using LambdaAlexaLastDance2.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]


namespace LambdaAlexaLastDance2;

public class Function
{
    ILambdaLogger log;
    private RepositoryPeliculas repo;

    public Function()
    {
        var provider = Startup.ConfigureServices();
        this.repo = provider.GetService<RepositoryPeliculas>();
        //var services = new ServiceCollection();
        //ConfigureServices(services);
        //ServiceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<RepositoryPeliculas>();
        string connectionString = @"";
        services.AddDbContext<CineContext>
            (options => options.UseMySQL(connectionString));
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
    {
        SkillResponse response = new SkillResponse();
        //CREAMOS UN NUEVO CUERPO PARA LA RESPUESTA
        response.Response = new ResponseBody();
        //INDICAMOS SI FINALIZA EN LA SESIÓN DEL CHAT
        response.Response.ShouldEndSession = false;
        //LA RESPUESTA QUE IREMOS CONFIGURANDO EN PALABRAS PARA EL CHAT
        IOutputSpeech innerResponse = null;
        //CONFIGURAMOS CLOUDWATCH
        this.log = context.Logger;
        //ESCRIBIMOS LA PETICIÓN PARA VER SI FUNCIONA EN CLOUDWATCH
        log.LogLine("SkillRequest: " + JsonConvert.SerializeObject(input));
        //HEMOS VISTO QUE TENEMOS UNA FRASE DE INICIO (lunes santidad)
        //DICHA FRASE ES UN LaunchRequest PARA VER QUE HACEMOS A CONTINUACION
        //O QUE PERSONALIZAMOS LA PRIMERA VEZ QUE NOS INVOCAN
        if (input.GetRequestType() == typeof(LaunchRequest))
        {
            //PRIMERA PETICION AL CHAT Y ESCRIBIMOS UN TEXTO
            innerResponse = new PlainTextOutputSpeech();
            //ESCRIBIMOS EL TEXTO DE SALIDA
            (innerResponse as PlainTextOutputSpeech).Text = "Soy tu Alexa de lunes, " +
                "viva su santidad papa Leon XIV. ¿Quieres información de películas?";
        }
        else if (input.GetRequestType() == typeof(IntentRequest))
        {
            //DEBEMOS LOCALIZAR EL TIPO DE INTENT POR SU NAME
            var intentRequest = (IntentRequest)input.Request;
            //AQUI YA PREGUNTAMOS SOBRE EL ID DE NUESTRO INTENT
            if (intentRequest.Intent.Name == "peliculasactor")
            {
                log.LogLine("Nos están pidiendo datos!!!");
                //RECUPERAMOS LA INFORMACIÓN DEL SLOT/VARIABLES QUE VIENE CON EL INTENT
                string slotJson = JsonConvert.SerializeObject(intentRequest.Intent.Slots);
                //RECUPERAMOS EL ID DE LA PELICULA
                string actor = this.GetSlotValueActorPelicula(slotJson);
                //BUSCAMOS LA PELICULA
                List<Pelicula> pelis = await repo.GetPeliculasByActorAsync(actor);
                //CREAMOS UNA RESPUESTA 
                innerResponse = new PlainTextOutputSpeech();
                if (pelis == null)
                {
                    //LA PELI NO EXISTE
                    (innerResponse as PlainTextOutputSpeech).Text =
                        "No existen peliculas con el actor: " + actor;
                }
                else
                {
                    (innerResponse as PlainTextOutputSpeech).Text = "Las pelis con el actor " + actor + " son las siguientes: ";
                    foreach (Pelicula peli in pelis)
                    {
                        (innerResponse as PlainTextOutputSpeech).Text = "Titulo: "
                            + peli.Titulo + ", Argumento: " + peli.Argumento;
                    }
                }
            }
        }
        else
        {
            //SI NO ENTRA EN NUESTRO INTENT, DEVOLVEMOS LO QUE SEA
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text = "Ni idea de lo que me estas diciendo jai...";
        }
        //DAMOS SALIDA PARA TESTEAR
        response.Response.OutputSpeech = innerResponse;
        response.Version = "1.0";
        return response;
    }

    public string GetSlotValueActorPelicula(string dataJson)
    {
        //CREAMOS UN OJETO JSON CON NEWTON
        var jsonObject = JObject.Parse(dataJson);
        var data = (JObject)jsonObject["actor"];
        var nombre = (string)data["name"];
        var actores = (string)data["value"];
        return actores;
    }
}
