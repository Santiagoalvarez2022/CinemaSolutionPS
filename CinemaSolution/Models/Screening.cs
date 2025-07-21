

namespace CinemaSolution.Models
{
    public class Screening
    {
        public int Id { get; }
        public decimal Price { get; }
        public DateTime StartScreening { get; }
        public DateTime FinishScreening { get; }
        public int IdMovie { get; }
        public int IdDirector { get; }
        public bool IsInternational { get; }

        public Screening(int id, decimal price, DateTime startScreening, DateTime finishScreening, int idMovie, int idDirector, bool isInternational)
        {
            Id = id;
            Price = price;
            StartScreening = startScreening;
            FinishScreening = finishScreening;
            IdMovie = idMovie;
            IdDirector = idDirector;
            IsInternational = isInternational;
        }

        public override string ToString()
        {
            var isInternational = IsInternational.ToString().ToLower();
            return  $"{Id}|{Price}|{StartScreening:yyyy-MM-dd HH:mm}|{FinishScreening:yyyy-MM-dd HH:mm}|{IdMovie}|{IdDirector}|{isInternational}" ;
        }
    }
}