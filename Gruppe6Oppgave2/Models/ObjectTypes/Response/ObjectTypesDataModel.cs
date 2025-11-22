using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.ObjectTypes.Response;

// Data modell for respons som inneholder objekt typer
public class ObjectTypesDataModel
{
    // Liste over tilgjengelige objekt typer
    public List<ObjectType> ObjectTypes { get; set; } = [];

    // @NOTE: Diksjonær nøkkel må være av typen int for at JSON skal serialiseres korrekt til JavaScript,
    //        ellers blir nøkkelen tolket som en streng.
    public Dictionary<int, Guid> StandardTypeIds { get; set; } = [];

    // Data modell for en enkelt objekt type
    public class ObjectType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; } // Valgfri URL til et bilde som representerer objekt typen
        public string? Colour { get; set; } // Valgfri fargekode for objekt typen
        public GeometryType GeometryType { get; set; }
    }
}
