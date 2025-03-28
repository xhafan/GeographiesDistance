using NetTopologySuite.IO;
using NetTopologySuite;
using NUnit.Framework;
using Npgsql;
using Shouldly;

namespace GeographiesDistance;

[TestFixture]
public class when_calculating_geography_distance
{
    private string _connectionString = null!;
    private NpgsqlConnection _connection = null!;
    private NpgsqlDataSource _dataSource = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _connectionString = "Host=localhost;Database=distance_calculator;Username=distance_calculator;Password=password01;";

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.UseNetTopologySuite();
        _dataSource = dataSourceBuilder.Build();

        using (var connection = _dataSource.CreateConnection())
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE EXTENSION IF NOT EXISTS postgis";
                command.ExecuteNonQuery();
            }
        }

        dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        dataSourceBuilder.UseNetTopologySuite();
        _dataSource = dataSourceBuilder.Build();
    }

    [SetUp]
    public void Context()
    {
        _connection = _dataSource.CreateConnection();
        _connection.Open();
    }

    [TestCase(
        "POLYGON ((14.2501 50.05023935110348, 14.250208509305542 50.050220681545085, 14.250287943430545 50.05016967540001, 14.250317018065362 50.05009999979769, 14.250287942884821 50.05003032429653, 14.25020850875982 50.04997931835377, 14.2501 50.04996064889653, 14.24999149124018 50.04997931835377, 14.249912057115178 50.05003032429653, 14.24988298193464 50.05009999979769, 14.249912056569457 50.05016967540001, 14.249991490694457 50.050220681545085, 14.2501 50.05023935110348))",
        "LINESTRING (14.4285094 50.0882756, 14.4282467 50.08762, 14.4152461 50.0851708, 14.4095242 50.0867578, 14.39508 50.08354, 14.3926231 50.0838461)",
        10857.27,
        TestName = "1 Polygon and linestring")]
    [TestCase(
        "POLYGON ((14.402678899999998 50.08715464110803, 14.403232372395449 50.087059485526765, 14.403637537109827 50.086799516607236, 14.403785830585274 50.08644439473768, 14.403637522904482 50.08608927549927, 14.403232358190106 50.08582931184206, 14.402678899999998 50.08573415889195, 14.40212544180989 50.08582931184206, 14.401720277095514 50.08608927549927, 14.401571969414723 50.08644439473768, 14.401720262890171 50.086799516607236, 14.402125427604547 50.087059485526765, 14.402678899999998 50.08715464110803))",
        "LINESTRING (14.4285094 50.0882756, 14.4282467 50.08762, 14.4152461 50.0851708, 14.4095242 50.0867578, 14.39508 50.08354, 14.3926231 50.0838461)",
        49.7,
        TestName = "2 Polygon and linestring")]
    public void distance_is_correct(
        string geographyOneWkt,
        string geographyTwoWkt,
        double expectedDistanceInMeters
    )
    {
        var wktReader = new WKTReader(NtsGeometryServices.Instance);
        var geographyOne = wktReader.Read(geographyOneWkt);
        var geographyTwo = wktReader.Read(geographyTwoWkt);
        var distanceCalculator = new PostgresGeographyDistanceCalculator(_connection);

        var distanceInMeters = distanceCalculator.CalculateDistanceInMeters(geographyOne, geographyTwo);

        distanceInMeters.ShouldBe(expectedDistanceInMeters, tolerance: expectedDistanceInMeters / 100);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Close();
    }
}