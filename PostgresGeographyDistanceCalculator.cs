using NetTopologySuite.Geometries;
using Npgsql;

namespace GeographiesDistance;

public class PostgresGeographyDistanceCalculator(NpgsqlConnection connection)
{
    public double CalculateDistanceInMeters(Geometry geographyOne, Geometry geographyTwo)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT ST_DistanceSphere(:geographyOne, :geographyTwo)";
        command.Parameters.AddWithValue("geographyOne", geographyOne);
        command.Parameters.AddWithValue("geographyTwo", geographyTwo);

        return (command.ExecuteScalar() as double?)!.Value;
    }
}