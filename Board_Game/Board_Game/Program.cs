// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;

interface IKlasaPostaci
{
    string KlasaNazwa { get; }          // Nazwa klasy postaci
    void AktywujZdolnoscSpecjalna(Player aktywujacy, List<Player> gracze, int tura);
    int Cooldown { get; }  // Czas cooldownu w turach
    int OstatniaAktywacja { get; set; }  // Numer ostatniej aktywacji
}

class Wojownik : IKlasaPostaci
{
    public string KlasaNazwa => "Wojownik";
    public int Cooldown => 4;  // Czas cooldownu wynosi 4 tury
    public int OstatniaAktywacja { get; set; } = -4;  // Ostatnia aktywacja 4 tury przed pierwszą

    public void AktywujZdolnoscSpecjalna(Player aktywujacy, List<Player> gracze, int tura)
    {
        if (tura - OstatniaAktywacja < Cooldown)
        {
            Console.WriteLine("Zdolność specjalna jest na cooldownie! Pozostało tur: " + (Cooldown - (tura - OstatniaAktywacja)));
            return;
        }

        Console.WriteLine("Wybierz gracza, którego chcesz zaatakować:");
        for (int i = 0; i < gracze.Count; i++)
        {
            if (gracze[i] != aktywujacy)
                Console.WriteLine($"{i + 1}. {gracze[i].Name} ({gracze[i].Score} punktów)");
        }

        int wybor = int.Parse(Console.ReadLine()) - 1;
        if (wybor < 0 || wybor >= gracze.Count || gracze[wybor] == aktywujacy)
        {
            Console.WriteLine("Nieprawidłowy wybór.");
            return;
        }

        var cel = gracze[wybor];
        int odebranePunkty = Math.Max(1, cel.Score * new Random().Next(1, 11) / 100);
        cel.UpdateScore(-odebranePunkty);
        aktywujacy.UpdateScore(odebranePunkty);

        Console.WriteLine($"{aktywujacy.Name} odebrał {odebranePunkty} punktów graczowi {cel.Name}.");
        OstatniaAktywacja = tura;  // Resetujemy ostatnią aktywację
    }
}

class Mag : IKlasaPostaci
{
    public string KlasaNazwa => "Mag";
    public int Cooldown => 2;
    public int OstatniaAktywacja { get; set; } = -2;

    public void AktywujZdolnoscSpecjalna(Player aktywujacy, List<Player> gracze, int tura)
    {
        if (tura - OstatniaAktywacja < Cooldown)
        {
            Console.WriteLine("Zdolność specjalna jest na cooldownie! Pozostało tur: " + (Cooldown - (tura - OstatniaAktywacja)));
            return;
        }

        int dodatkowyRzut = aktywujacy.RzucKostka();
        Console.WriteLine($"{aktywujacy.Name} rzucił ponownie: {dodatkowyRzut}");
        aktywujacy.Przemiesc(dodatkowyRzut);
        OstatniaAktywacja = tura;
    }
}

class Healer : IKlasaPostaci
{
    public string KlasaNazwa => "Healer";
    public int Cooldown => 3;
    public int OstatniaAktywacja { get; set; } = -3;

    public void AktywujZdolnoscSpecjalna(Player aktywujacy, List<Player> gracze, int tura)
    {
        if (tura - OstatniaAktywacja < Cooldown)
        {
            Console.WriteLine("Zdolność specjalna jest na cooldownie! Pozostało tur: " + (Cooldown - (tura - OstatniaAktywacja)));
            return;
        }

        Console.WriteLine("Wybierz gracza, który może rzucić ponownie kostką:");
        for (int i = 0; i < gracze.Count; i++)
        {
            if (gracze[i] != aktywujacy)
                Console.WriteLine($"{i + 1}. {gracze[i].Name}");
        }

        int wybor = int.Parse(Console.ReadLine()) - 1;
        if (wybor < 0 || wybor >= gracze.Count || gracze[wybor] == aktywujacy)
        {
            Console.WriteLine("Nieprawidłowy wybór.");
            return;
        }

        var cel = gracze[wybor];
        int dodatkowyRzut = cel.RzucKostka();
        Console.WriteLine($"{cel.Name} rzucił ponownie: {dodatkowyRzut}");
        cel.Przemiesc(dodatkowyRzut);

        int bonusPunkty = Math.Max(1, cel.Score * new Random().Next(1, 11) / 100);
        aktywujacy.UpdateScore(bonusPunkty);

        Console.WriteLine($"{aktywujacy.Name} zdobył {bonusPunkty} punktów za leczenie gracza {cel.Name}.");
        OstatniaAktywacja = tura;
    }
}

class Player
{
    public string Name { get; set; }
    public (int x, int y) Position { get; private set; } = (1, 1);
    public int Score { get; private set; }
    private (int x, int y) MapSize;

    public IKlasaPostaci KlasaPostaci { get; private set; }

    private readonly Random rnd = new Random();

    public void SetMapSize((int x, int y) size) => MapSize = size;

    public void SetKlasaPostaci(IKlasaPostaci klasaPostaci) => KlasaPostaci = klasaPostaci;

    public void Move()
    {
        int dice = RzucKostka();
        Console.WriteLine($"{Name} wyrzucił: {dice}");
        Przemiesc(dice);
    }

    public int RzucKostka() => rnd.Next(1, 7);

    public void Przemiesc(int kroki)
    {
        Position = (Position.x + kroki, Position.y);
        if (Position.x > MapSize.x)
            Position = (1, Position.y + 1);

        Console.WriteLine($"{Name} teraz na pozycji: {Position.x}, poziom: {Position.y}");
    }

    public void UpdateScore(int delta)
    {
        Score += delta;
        Console.WriteLine($"{Name} ma teraz {Score} punktów.");
    }
}

class Board
{
    public (int x, int y) MapSize { get; } = (10, 10); // Przykładowy rozmiar planszy 10x10

    public List<(int x, int y)> GetRewards(int liczbaNagrod)
    {
        List<(int x, int y)> rewards = new List<(int x, int y)>();
        Random rnd = new Random();
        for (int i = 0; i < liczbaNagrod; i++)
        {
            int x = rnd.Next(1, MapSize.x + 1);
            int y = rnd.Next(1, MapSize.y + 1);
            rewards.Add((x, y));
        }
        return rewards;
    }
}

class Game
{
    private readonly List<Player> players = new();
    private readonly Board board = new();
    private readonly List<(int x, int y)> rewardsCoords;
    private int currentPlayerIndex = 0;
    private int tura = 0;

    public Game()
    {
        rewardsCoords = board.GetRewards(25);
        InitPlayers();
    }

    private void InitPlayers()
    {
        Console.WriteLine("Podaj liczbę graczy:");
        int playerCount = int.Parse(Console.ReadLine());

        for (int i = 1; i <= playerCount; i++)
        {
            Console.WriteLine($"Gracz {i}, podaj swoje imię:");
            string playerName = Console.ReadLine();

            Console.WriteLine($"Wybierz klasę: Wojownik, Mag, Healer");
            string inputKlasy = Console.ReadLine();

            IKlasaPostaci klasaPostaci = inputKlasy switch
            {
                "Wojownik" => new Wojownik(),
                "Mag" => new Mag(),
                "Healer" => new Healer(),
                _ => throw new Exception("Nieznana klasa postaci!")
            };

            var player = new Player { Name = playerName };
            player.SetKlasaPostaci(klasaPostaci);
            player.SetMapSize(board.MapSize);
            players.Add(player);
        }
    }

    public void Start()
    {
        while (!IsGameOver())
        {
            NextTurn();
            tura++;
        }

        DisplayResults();
    }

    private void NextTurn()
    {
        var player = players[currentPlayerIndex];
        Console.WriteLine($"\nTura gracza: {player.Name} ({player.KlasaPostaci.KlasaNazwa})");

        player.Move();

        if (rewardsCoords.Remove(player.Position))
        {
            Console.WriteLine($"{player.Name} zdobył nagrodę!");
            player.UpdateScore(10);
        }

        // Sprawdzamy, czy zdolność specjalna jest dostępna
        if (tura - player.KlasaPostaci.OstatniaAktywacja >= player.KlasaPostaci.Cooldown)
        {
            Console.WriteLine($"Czy chcesz aktywować zdolność specjalną? (tak/nie)");
            if (Console.ReadLine()?.ToLower() == "tak")
                player.KlasaPostaci.AktywujZdolnoscSpecjalna(player, players, tura);
        }
        else
        {
            Console.WriteLine("Zdolność specjalna na cooldownie!");
        }

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
    }

    private bool IsGameOver() => players.Exists(p => p.Position.y > board.MapSize.y);

    private void DisplayResults()
    {
        Console.WriteLine("\nGra zakończona! Wyniki końcowe:");
        foreach (var player in players)
        {
            Console.WriteLine($"{player.Name}: {player.Score} punktów");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Start();
    }
}
