using CsvHelper.Configuration;

namespace golden.raspberry.awards.api.Domain.Entities
{
    public class MovieAwardNomination : BaseEntity
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public string Studio { get; set; }
        public string Producer { get; set; }
        public string Winner { get; set; }
    }

    public class MovieAwardNominationMap : ClassMap<MovieAwardNomination>
    {
        public MovieAwardNominationMap()
        {
            Map(m => m.Year).Name("year");
            Map(m => m.Title).Name("title");
            Map(m => m.Studio).Name("studios");
            Map(m => m.Producer).Name("producers");
            Map(m => m.Winner).Name("winner");
        }
    }
}