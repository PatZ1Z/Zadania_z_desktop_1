// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;




    // Interfejs definiujący gracza
    public interface IPlayer
    {
        string Name { get; set; }
        string Position { get; set; }
        int Score { get; set; }
        
        void UpdateScore(int points); // Dodaj metodę do interfejsu
    }

    // Klasa Player implementująca interfejs IPlayer
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public int Score { get; set; }

        // Metoda do aktualizacji wyniku
        public void UpdateScore(int points)
        {
            Score += points;
            Console.WriteLine($"{Name} zdobył {points} punktów. Nowy wynik: {Score}");
        }
    }

    // Klasa Team zarządzająca drużyną
    public class Team
    {
        private List<IPlayer> players = new List<IPlayer>();

        // Metoda do dodawania zawodnika
        public void AddPlayer(IPlayer player)
        {
            if (player == null || string.IsNullOrWhiteSpace(player.Name) || string.IsNullOrWhiteSpace(player.Position))
            {
                Console.WriteLine("Nie można dodać zawodnika: niepełne dane.");
                return;
            }

            players.Add(player);
            Console.WriteLine($"Dodano zawodnika: {player.Name}");
        }

        // Metoda do usuwania zawodnika
        public void RemovePlayer(string name)
        {
            var player = players.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                players.Remove(player);
                Console.WriteLine($"Usunięto zawodnika: {name}");
            }
            else
            {
                Console.WriteLine($"Nie znaleziono zawodnika o imieniu: {name}");
            }
        }

        // Metoda do wyświetlania zawodników
        public void DisplayTeam()
        {
            if (!players.Any())
            {
                Console.WriteLine("Drużyna jest pusta.");
                return;
            }

            Console.WriteLine("Zawodnicy w drużynie:");
            foreach (var player in players)
            {
                Console.WriteLine($"- {player.Name} ({player.Position}), Wynik: {player.Score}");
            }
        }

        // Metoda do obliczania średniej punktów zdobytych przez drużynę
        public static double CalculateAverageScore(List<IPlayer> players)
        {
            if (players == null || players.Count == 0) return 0;
            return players.Average(p => p.Score);
        }

        // Metoda do wyświetlania statystyk drużyny
        public void DisplayStatistics()
        {
            double averageScore = CalculateAverageScore(players);
            Console.WriteLine($"Średnia punktów zdobytych przez drużynę: {averageScore:F2}");
        }

        // Metoda do filtrowania zawodników na podstawie pozycji
        public void FilterPlayersByPosition(string position)
        {
            var filteredPlayers = players.Where(p => p.Position.Equals(position, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!filteredPlayers.Any())
            {
                Console.WriteLine($"Brak zawodników na pozycji: {position}");
                return;
            }

            Console.WriteLine($"Zawodnicy na pozycji {position}:");
            foreach (var player in filteredPlayers)
            {
                Console.WriteLine($"- {player.Name}, Wynik: {player.Score}");
            }
        }

        // Delegaty i anonimowe funkcje do filtrowania zawodników
        public List<IPlayer> FilterPlayers(Func<IPlayer, bool> criteria)
        {
            var filteredPlayers = players.Where(criteria).ToList();
            if (!filteredPlayers.Any())
            {
                Console.WriteLine("Brak zawodników spełniających kryteria.");
            }

            return filteredPlayers;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Team team = new Team();

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Dodaj zawodnika");
                Console.WriteLine("2. Usuń zawodnika");
                Console.WriteLine("3. Wyświetl drużynę");
                Console.WriteLine("4. Aktualizuj wynik zawodnika");
                Console.WriteLine("5. Wyświetl statystyki drużyny");
                Console.WriteLine("6. Wyszukaj zawodników według pozycji");
                Console.WriteLine("7. Filtrowanie zawodników");
                Console.WriteLine("8. Wyjdź");
                Console.Write("Wybierz opcję: ");
                int choice = int.Parse(Console.ReadLine() ?? "8");

                switch (choice)
                {
                    case 1:
                        Console.Write("Imię zawodnika: ");
                        string name = Console.ReadLine();
                        Console.Write("Pozycja zawodnika: ");
                        string position = Console.ReadLine();
                        Console.Write("Początkowy wynik: ");
                        int score = int.Parse(Console.ReadLine() ?? "0");
                        team.AddPlayer(new Player { Name = name, Position = position, Score = score });
                        break;

                    case 2:
                        Console.Write("Imię zawodnika do usunięcia: ");
                        string nameToRemove = Console.ReadLine();
                        team.RemovePlayer(nameToRemove);
                        break;

                    case 3:
                        team.DisplayTeam();
                        break;

                    case 4:
                        Console.Write("Imię zawodnika do aktualizacji: ");
                        string playerName = Console.ReadLine();
                        List<IPlayer> playerToUpdate = team.FilterPlayers(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
                        if (playerToUpdate != null)
                        {
                            Console.Write("Ile punktów dodać? ");
                            int points = int.Parse(Console.ReadLine() ?? "0");
                            foreach (var player in playerToUpdate)
                                player.UpdateScore(points);
                        }
                        break;

                    case 5:
                        team.DisplayStatistics();
                        break;

                    case 6:
                        Console.Write("Podaj pozycję: ");
                        string pos = Console.ReadLine();
                        team.FilterPlayersByPosition(pos);
                        break;

                    case 7:
                        Console.Write("Filtrowanie według minimalnych punktów: ");
                        int minScore = int.Parse(Console.ReadLine() ?? "0");
                        team.FilterPlayers(p => p.Score >= minScore);
                        break;

                    case 8:
                        return;

                    default:
                        Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                        break;
                }
            }
        }
    }
