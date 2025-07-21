
namespace CinemaSolution.Models
{
    public class Movie
    {
        public int Id { get; }
        public string Name { get; }
        public int Duration { get; }
        public int IdDirector { get; }
        public bool IsInternational { get; }

        public Movie(int id, string name, int duration, int idDirector, bool isInternational)
        {
            Id = id;
            Name = name;
            Duration = duration;
            IdDirector = idDirector;
            IsInternational = isInternational;
        }

        public override string ToString()
        {
            var isInternational = IsInternational.ToString().ToLower();
            return $"{Id}|{Name}|{Duration}|{IdDirector}|{isInternational}";
        }
    }
}