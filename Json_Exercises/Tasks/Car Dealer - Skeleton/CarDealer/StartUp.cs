using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            {
                var inputJson = File.ReadAllText("./../../../Datasets/cars.json");
                
                using (var db = new CarDealerContext())
                {
                    var result = ImportCars(db, inputJson);
                    Console.WriteLine(result);
                }
            }
        }
        //Problem 9.
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var jsonOutput = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.Suppliers.AddRange(jsonOutput);
            var count = context.SaveChanges();
            return $"Successfully imported {count}.";
        }
        //Problem 10.
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var jsonOutput = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(p => p.SupplierId <= 31);

            context.Parts.AddRange(jsonOutput);
            context.SaveChanges();
            return $"Successfully imported {jsonOutput.Count()}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            //var jsonOutput = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            //foreach (var carDto in jsonOutput)
            //{
            //    var car = new Car
            //    {
            //        Make = carDto.Make,
            //        Model = carDto.Model,
            //        TravelledDistance = carDto.TravelledDistance
            //    };
            //    context.Cars.Add(car);

            //    foreach (var partId in carDto.PartsId)
            //    {
            //        PartCar partCar = new PartCar
            //        {
            //            CarId = car.Id,
            //            PartId = partId
            //        };

            //        if (car.PartCars.FirstOrDefault(p => p.PartId == partId) == null)
            //        {
            //            context.PartCars.Add(partCar);
            //        }
            //    }
            //}
            //context.SaveChanges();

            //return $"Successfully imported { jsonOutput.Count()}.";
            namespace QuickType
    {
        using System;
        using System.Collections.Generic;

        using System.Globalization;
        using Newtonsoft.Json;
        using Newtonsoft.Json.Converters;

        public partial class Welcome
        {
            [JsonProperty("firstName")]
            public string FirstName { get; set; }

            [JsonProperty("lastName")]
            public string LastName { get; set; }

            [JsonProperty("age")]
            [JsonConverter(typeof(DecodingChoiceConverter))]
            public long? Age { get; set; }
        }

        public partial class Welcome
        {
            public static Welcome[] FromJson(string json) => JsonConvert.DeserializeObject<Welcome[]>(json, QuickType.Converter.Settings);
        }

        public static class Serialize
        {
            public static string ToJson(this Welcome[] self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        }

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

        internal class DecodingChoiceConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(long?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Null:
                        return null;
                    case JsonToken.Integer:
                        var integerValue = serializer.Deserialize<long>(reader);
                        return integerValue;
                    case JsonToken.String:
                    case JsonToken.Date:
                        var stringValue = serializer.Deserialize<string>(reader);
                        long l;
                        if (Int64.TryParse(stringValue, out l))
                        {
                            return l;
                        }
                        break;
                }
                throw new Exception("Cannot unmarshal type long?");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                var value = (long?)untypedValue;
                if (value == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                if (value != null)
                {
                    serializer.Serialize(writer, value.Value);
                    return;
                }
                throw new Exception("Cannot marshal type long?");
            }

            public static readonly DecodingChoiceConverter Singleton = new DecodingChoiceConverter();
        }
    }


}
    }
    
}