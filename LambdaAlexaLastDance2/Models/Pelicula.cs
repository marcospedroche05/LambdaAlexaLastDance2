using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LambdaAlexaLastDance2.Models
{
    [Table("pelismysql")]
    public class Pelicula
    {
        [Key]
        [Column("idPelicula")]
        public int IdPelicula { get; set; }
        [Column("Genero")]
        public string Genero { get; set; }
        [Column("Titulo")]
        public string Titulo { get; set; }
        [Column("Actores")]
        public string Actores { get; set; }
        [Column("Argumento")]
        public string Argumento { get; set; }
        [Column("Precio")]
        public int Precio { get; set; }
        [Column("YouTube")]
        public string YouTube { get; set; }
    }
}
