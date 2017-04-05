using Itinero;
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;
using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace ConsoleApp1
{
    class Program
    {
        private const string UkRoutingFilePath = @"/App_Data/osmfile_uk.routing";
        static void Main(string[] args)
        {
            var p = AppDomain.CurrentDomain.BaseDirectory;
            var routerDb = new RouterDb();
            if (!File.Exists(p + UkRoutingFilePath))
            {
                using (var stream = new FileInfo(p + @"/App_Data/british-isles-latest.osm.pbf").OpenRead())
                {
                    routerDb.LoadOsmData(stream, Vehicle.Car);
                }

                using (var stream = new FileInfo(p + UkRoutingFilePath).Open(FileMode.Create, FileAccess.ReadWrite))
                {
                    routerDb.Serialize(stream);
                }
            }
            else
            {
                var stream = new FileInfo(p + UkRoutingFilePath).OpenRead();
                routerDb = RouterDb.Deserialize(stream, RouterDbProfile.NoCache);
            }

            var router = new Router(routerDb);
            var route = router.Calculate(Vehicle.Car.Fastest(), 52.4247f, -3.7933f, 52.4427f, -3.5347f);

            var distance = float.Parse(route.Attributes.First(a => a.Key == "distance").Value) / 1000;
            var time = float.Parse(route.Attributes.First(a => a.Key == "time").Value) / 60;

            WriteLine($"distance: {distance:####.##} Km");
            WriteLine($"time: {time:####} min");
            ReadLine();
        }
    }
}
