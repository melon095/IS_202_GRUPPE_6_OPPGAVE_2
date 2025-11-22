using Gruppe6Oppgave2.Database.Tables;

namespace Gruppe6Oppgave2.Models.Report.Response;

// Modell for et punkt med latitude og longitude
public struct Point
{
    public Guid Id { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }

    // Konstruktør for å initialisere et punkt med latitude og longitude
    public Point(double lat, double lng)
    {
        Lat = lat;
        Lng = lng;
    }

    // Konstruktør for å initialisere et punkt fra en HindrancePointTable
    public Point(HindrancePointTable point)
    {
        Id = point.Id;
        Lat = point.Latitude;
        Lng = point.Longitude;
    }
}
