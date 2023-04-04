namespace golden.raspberry.awards.api.Domain.Entities
{
    public class ProducerAward
    {
        public string Producer { get; set; }
        public int Interval { get; set; }
        public int PreviusWin { get; set; }
        public int FollowingWin { get; set; }
    }
}
