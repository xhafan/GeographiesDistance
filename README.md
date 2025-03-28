GeographiesDistance project contains:
* `PostgresGeographyDistanceCalculator` which calculates
distance in meters between two geographies using postgres/postgis. 
* ``when_calculating_geography_distance`` tests.

To run the tests, do this:
1. Install postgres/postgis
2. Execute `db-init.sql` under a super user, to create a new super user `distance_calculator` and a new database `distance_calculator`

If anybody knows C# solution for calculating a distance between two geographies, please let me know on 
SO: https://stackoverflow.com/questions/79539184/calculate-earth-distance-between-geography-polygons-in-net
