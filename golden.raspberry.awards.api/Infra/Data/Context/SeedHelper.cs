using CsvHelper;
using CsvHelper.Configuration;
using golden.raspberry.awards.api.Domain.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace golden.raspberry.awards.api.Infra.Data.Context
{
    public static class SeedHelper
    {
        public static List<MovieAwardNomination> GetMovieAwardNominations(string delimiter, string filePath)
        {
            var movieAwards = new List<MovieAwardNomination>();
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = delimiter };
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, csvConfig))
            {
                csv.Context.RegisterClassMap<MovieAwardNominationMap>();
                movieAwards = csv.GetRecords<MovieAwardNomination>().ToList();
            }

            return movieAwards;
        }
    }
}
