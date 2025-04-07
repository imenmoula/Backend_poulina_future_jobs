using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class Competence
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime DateModification { get; set; } = DateTime.UtcNow;

        // Listes d'énumérations pour HardSkills et SoftSkills
        public List<HardSkillType> HardSkills { get; set; } = new List<HardSkillType>();
        public List<SoftSkillType> SoftSkills { get; set; } = new List<SoftSkillType>();

        // Many-to-many relationship with OffreEmploi via OffreCompetences
        public virtual ICollection<OffreCompetences> OffreCompetences { get; set; } = new List<OffreCompetences>();
    }

    public enum HardSkillType
    {
        Csharp,
        DotNetCore,
        Java,
        Python,
        JavaScript,
        TypeScript,
        React,
        Angular,
        VueJS,
        HTML,
        CSS,
        SQL,
        NoSQL,
        Git,
        Docker,
        Kubernetes,
        DevOps,
        CloudComputing,
        MachineLearning,
        DataScience,
        CyberSecurity,
        UIUXDesign,
        MobileDevelopment,
        GameDevelopment,
        Blockchain,
        InternetOfThings,
        BigData,
        VirtualReality,
        AugmentedReality,
       Pandas,
        PowerBI,
        Scrum,
        Agile,
        ProjectManagement,
        SoftwareTesting,
        ExpressJS,
        NodeJS,
        
    }

    public enum SoftSkillType
    {
        Discipline,
        EcouteActive,
        Communication,
        TravailEquipe,
        GestionTemps,
        Adaptabilite,
        EspritCritique,
        Empathie,
        Creativite,
        Leadership,
        GestionConflits,
        Negociation,
        PriseDecisions,
        GestionStress,
        SensibiliteCulturelle,
        IntelligenceEmotionnelle,
        GestionChangement,
        VisionStrategique,
        OrientationResultats,
        GestionProjets,
        Innovation,
        Responsabilite,
        Autonomie,
        Curiosite,
        EspritAnalytique,
        SensibiliteClient,
        GestionRisques,
        GestionQualite,
        GestionConnaissances,
        AttentionAuxDetails,
        GestionDesPerformances,
        Collaboration,
       


    }
}