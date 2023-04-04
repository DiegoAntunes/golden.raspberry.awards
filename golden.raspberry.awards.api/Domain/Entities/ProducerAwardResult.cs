using System.Collections.Generic;

namespace golden.raspberry.awards.api.Domain.Entities
{
    public class ProducerAwardResult
    {
        public List<ProducerAward> Min { get; set; }
        public List<ProducerAward> Max { get; set; }
    }
}