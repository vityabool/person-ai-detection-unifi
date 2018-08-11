using System;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using helper;


namespace person_ai_detection_unifi
{
    class Program
    {

        static void Main(string[] args)
        {
            // Handle Ctrl+C 
            Console.CancelKeyPress += delegate {
                Helper.Log("Application termination signal received. Press Enter to exit");
                Console.ReadLine();
                Environment.Exit(0);
            };

            // Read App configuration
            Helper.ReadConfiguration();

            // DEBUG
            Int64 testStartTime = 1533903000000; // 08/10/2018 @ 12:10pm (UTC)
            Int64 debugTimeDiff = 111178000;

            // Starting 
            DateTime curretTime = DateTime.UtcNow;
            var mogoCurrentTime = Helper.DateTimeToMongoTimeStamp(curretTime);

            //mogoCurrentTime = testStartTime;

            Console.WriteLine($"Starting to lookup for new videos: {curretTime.ToUniversalTime()} Mongo TimeStamp: {mogoCurrentTime}");
            
            // Connecting to MongoDB
            MongoClient mongo = new MongoClient(Helper.Configuration["MongoDB_ConnectionString"]);
            IMongoDatabase av = mongo.GetDatabase(Helper.Configuration["MongoDB_DBName"]);
            var events = av.GetCollection<BsonDocument>(Helper.Configuration["MongoDB_Collection"]);


            Helper.DEBUG($"Testing variable time: {mogoCurrentTime - debugTimeDiff} vs. fixed testing time: {testStartTime}");


            Helper.Log($"Starting to look for the new motion from {Helper.MongoTimeStampToDateTime(mogoCurrentTime).ToLocalTime()}");

            while (true)
            {

                var fltBuilder = Builders<BsonDocument>.Filter;
                var filter = fltBuilder.Gt("startTime", mogoCurrentTime); // & fltBuilder.Ne("inProgress", false);
                var documents = events.Find<BsonDocument>(filter).ToList();


                //if (documents.Count == 0)
                //    DEBUG($"No more new videos at {MongoTimeStampToDateTime(mogoCurrentTime).ToLocalTime()}");

                foreach (var doc in documents)
                {
                    var startTime = Helper.MongoTimeStampToDateTime(doc["startTime"].ToInt64());
                    var endTime = Helper.MongoTimeStampToDateTime(doc["endTime"].ToInt64());

                    Helper.Log($"New motion: {doc["_id"]} recording from {startTime} to {endTime}");

                    mogoCurrentTime = doc["startTime"].ToInt64();
                }

                System.Threading.Thread.Sleep(1000);
            }
    }
}
}
