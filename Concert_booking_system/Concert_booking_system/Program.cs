// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;


public interface IConcert
{
    string Name { get; set; }
    string Date { get; set; }
    string Location { get; set; }
    List<int> AvailableSeats { get; set; }
    int TicketPrice { get; }
    int InitialSeats { get; } // Początkowa liczba miejsc
}

public class RegularConcert : IConcert
{
    public string Name { get; set; }
    public string Date { get; set; }
    public string Location { get; set; }
    public List<int> AvailableSeats { get; set; }
    public int TicketPrice { get; set; }
    public int InitialSeats { get; set; }

    public RegularConcert(string name, string date, string location, List<int> availableSeats, int ticketPrice)
    {
        Name = name;
        Date = date;
        Location = location;
        AvailableSeats = availableSeats;
        TicketPrice = ticketPrice;
        InitialSeats = availableSeats.Count; // Ustawiamy początkową liczbę miejsc
    }
}

public class VIPConcert : IConcert
{
    public string Name { get; set; }
    public string Date { get; set; }
    public string Location { get; set; }
    public List<int> AvailableSeats { get; set; }
    public int TicketPrice { get; set; }
    public bool HasMeetAndGreet { get; set; }
    public int InitialSeats { get; set; }

    public VIPConcert(string name, string date, string location, List<int> availableSeats, int ticketPrice, bool hasMeetAndGreet)
    {
        Name = name;
        Date = date;
        Location = location;
        AvailableSeats = availableSeats;
        TicketPrice = ticketPrice;
        HasMeetAndGreet = hasMeetAndGreet;
        InitialSeats = availableSeats.Count; // Ustawiamy początkową liczbę miejsc
    }
}

public class OnlineConcert : IConcert
{
    public string Name { get; set; }
    public string Date { get; set; }
    public string Location { get; set; }
    public List<int> AvailableSeats { get; set; }
    public int TicketPrice { get; set; }
    public string StreamingPlatform { get; set; }
    public int InitialSeats { get; set; }

    public OnlineConcert(string name, string date, string location, List<int> availableSeats, int ticketPrice, string streamingPlatform)
    {
        Name = name;
        Date = date;
        Location = location;
        AvailableSeats = availableSeats;
        TicketPrice = ticketPrice;
        StreamingPlatform = streamingPlatform;
        InitialSeats = availableSeats.Count; // Ustawiamy początkową liczbę miejsc
    }
}

public class PrivateConcert : IConcert
{
    public string Name { get; set; }
    public string Date { get; set; }
    public string Location { get; set; }
    public List<int> AvailableSeats { get; set; }
    public int TicketPrice { get; set; }
    public bool ByInvitationOnly { get; set; }
    public int InitialSeats { get; set; }

    public PrivateConcert(string name, string date, string location, List<int> availableSeats, int ticketPrice, bool byInvitationOnly)
    {
        Name = name;
        Date = date;
        Location = location;
        AvailableSeats = availableSeats;
        TicketPrice = ticketPrice;
        ByInvitationOnly = byInvitationOnly;
        InitialSeats = availableSeats.Count; // Ustawiamy początkową liczbę miejsc
    }
}

class Concert
{
    
        public static List<IConcert> Concerts = new List<IConcert>();
        
        public static void NewConcert(IConcert concert)
        {
            Concerts.Add(concert);
            
        }

        public static void CheckSeats(IConcert concert)
        {
            double availablePercentage = (double)concert.AvailableSeats.Count / concert.InitialSeats * 100;
            if (availablePercentage < 30)
            {
                Console.WriteLine($"Warning: Less than 30% of seats are available for the concert {concert.Name}!");
            }
        }
        
}

class Ticket
{
    public string ConcertName { get; set; }
    public int Price { get; set; }
    public int SeatNumber { get; set; }
    public static List<Ticket> YourTickets = new List<Ticket>();

    public void SetTicket(string concertName, int price, int seatNumber)
    {
        YourTickets.Add(new Ticket { ConcertName = concertName, Price = price, SeatNumber = seatNumber });
    }
    
    public static void CancelTicket(string concertName, int seatNumber)
    {
        var ticket = YourTickets.FirstOrDefault(t => t.ConcertName == concertName && t.SeatNumber == seatNumber);
        if (ticket != null)
        {
            YourTickets.Remove(ticket);
            var concert = Concert.Concerts.FirstOrDefault(c => c.Name == concertName);
            concert?.AvailableSeats.Add(seatNumber); // Przywracamy miejsce
            Console.WriteLine($"Ticket for {concertName} on seat {seatNumber} has been cancelled.");
        }
        else
        {
            Console.WriteLine("Ticket not found.");
        }
    }
}

class BookingSystem
{
    public void AddNewConcert(IConcert concert)
    {
        Concert.NewConcert(concert);
    }

    public void BookTicket(string concertName, int price, int seatNumber)
    {
        var vipConcert = Concert.Concerts.OfType<VIPConcert>().FirstOrDefault(c => c.Name == concertName);
        var concert = Concert.Concerts.Find(c => c.Name == concertName);
       
        
        if (vipConcert != null) // Jeśli to koncert VIP
        {
            if (vipConcert.AvailableSeats.Contains(seatNumber))
            {
                Ticket ticket = new Ticket();
                ticket.SetTicket(concertName, price, seatNumber);
                vipConcert.AvailableSeats.Remove(seatNumber); // Usuwamy miejsce VIP
                Console.WriteLine($"VIP Ticket booked for {concertName}, Seat Number: {seatNumber}, Price: {price}");
                Concert.CheckSeats(concert);
            }
            else
            {
                Console.WriteLine("VIP seat is already booked.");
            }
        }
        else
        {
            // Jeśli to koncert nie-VIP, sprawdzamy inne typy koncertów
            
            if (concert != null && concert.AvailableSeats.Contains(seatNumber))
            {
                Ticket ticket = new Ticket();
                ticket.SetTicket(concertName, price, seatNumber);
                concert.AvailableSeats.Remove(seatNumber); // Usuwamy miejsce
                Console.WriteLine($"Ticket booked for {concertName}, Seat Number: {seatNumber}, Price: {price}");
                Concert.CheckSeats(concert);
            }
            else if (concert == null)
            {
                Console.WriteLine("No such concert available.");
            }
            else
            {
                Console.WriteLine("Seat already booked for this concert.");
            }
        }

        
    }

    public void ShowConcertsByDate(string date)
    {
        var concertsOnDate = Concert.Concerts.Where(c => c.Date == date).ToList();
        Console.WriteLine($"Concerts on {date}:");
        foreach (var concert in concertsOnDate)
        {
            Console.WriteLine($"- {concert.Name} at {concert.Location}");
        }
    }
    
    public void ShowConcertsByLocation(string location)
    {
        var concertsAtLocation = Concert.Concerts.Where(c => c.Location == location).ToList();
        Console.WriteLine($"Concerts at {location}:");
        foreach (var concert in concertsAtLocation)
        {
            Console.WriteLine($"- {concert.Name} on {concert.Date}");
        }
    }
    
    public void FilterConcerts(Func<IConcert, bool> filter)
    {
        var filteredConcerts = Concert.Concerts.Where(filter).ToList();
        foreach (var concert in filteredConcerts)
        {
            Console.WriteLine($"{concert.Name} on {concert.Date} at {concert.Location}");
        }
    }

    public void GenerateReport()
    {
        Console.WriteLine("Concerts report:");
        foreach (var concert in Concert.Concerts)
        {
            int soldTickets = Ticket.YourTickets.Count(t => t.ConcertName == concert.Name);
            Console.WriteLine($"Concert:{concert.Name} sold {soldTickets} tickets.");
        }
    }


}

internal class Program
{
    public static void Main(string[] args)
    {
        BookingSystem bookingSystem = new BookingSystem();

        // Adding concerts
        bookingSystem.AddNewConcert(new RegularConcert("Rock Concert", "2024-12-12", "Stadium", new List<int> { 1, 2, 3, 4, 5 }, 50));
        bookingSystem.AddNewConcert(new VIPConcert("VIP Concert", "2024-12-13", "Arena", new List<int> { 1, 2, 3 }, 200, true));
        bookingSystem.AddNewConcert(new OnlineConcert("Online Concert", "2024-12-14", "N/A", new List<int> { 1, 2, 3 }, 20, "YouTube"));
        bookingSystem.AddNewConcert(new PrivateConcert("Private Concert", "2024-12-15", "Private Location", new List<int> { 1, 2 }, 500, true));

        // Booking tickets
        bookingSystem.BookTicket("Rock Concert", 50, 1);
        bookingSystem.BookTicket("Rock Concert", 50, 2);
        bookingSystem.BookTicket("Rock Concert", 50, 3);
        bookingSystem.BookTicket("Rock Concert", 50, 4);

        
        
        
        
        
        bookingSystem.BookTicket("VIP Concert", 200, 1);

        // Display tickets booked
        foreach (var ticket in Ticket.YourTickets)
        {
            Console.WriteLine($"Concert: {ticket.ConcertName}, Seat: {ticket.SeatNumber}, Price: {ticket.Price}");
        }

        // Show concerts by date and location
        bookingSystem.ShowConcertsByDate("2024-12-12");
        bookingSystem.ShowConcertsByLocation("Stadium");

        // przefiltrowanie koncerty (np. VIP concerts only)
        bookingSystem.FilterConcerts(c => c is VIPConcert);
        
        //Generowanie raportu
        bookingSystem.GenerateReport();
    }
    
}