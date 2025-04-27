using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend_poulina_future_jobs.Models
{
    public class AppUserCompetence
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AppUserId { get; set; }
        public Guid CompetenceId { get; set; }

        public NiveauPossedeType NiveauPossede { get; set; }

        public AppUser AppUser { get; set; }
        public Competence Competence { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum NiveauPossedeType
    {
        [Display(Name = "Débutant")]
        Debutant = 0,

        [Display(Name = "Intermédiaire")]
        Intermediaire = 1,

        [Display(Name = "Avancé")]
        Avance = 2,

        [Display(Name = "Expert")]
        Expert = 3
    }
}