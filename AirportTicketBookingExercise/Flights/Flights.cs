using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AirportTicketBookingExercise.Users;

namespace AirportTicketBookingExercise.Flight
{

    #region Interfaces
    public interface IFlightPersistence
    {
        void SaveFlights(List<Flight>? flights, string path);
        List<Flight>? LoadFlights(string path);
    }

    public interface IBookingManager
    {
        bool BookFlight(User user, string flightId);
        bool CancelBooking(User user, string flightId);
        string? ModifyBookingClass(User user, string flightId, string classChoice);
    }

    public interface IFlightDisplay
    {
        void ShowFlights(List<Flight>? flights);
        bool ShowUserBookings(string? user);
    }

    public interface IFlightSearch
    {
        List<Flight>? SearchFlights(string searchTerm);
        List<Flight> FilterBookings(string searchTerm);
    }
    #endregion

    #region Implementations
    public class FlightPersistence : IFlightPersistence
    {
        public void SaveFlights(List<Flight>? flights, string path)
        {
            using StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII);
            foreach (var flight in flights)
            {
                sw.WriteLine(
                    $"{flight.Id}, {flight.Price}, {flight.DepartureCountry}, {flight.DestinationCountry}, {flight.DepartureDate}, {flight.DepartureAirport}, {flight.ArrivalAirport}, {flight.TicketClass}, {flight.Booked}, {(flight.Booked ? flight.Passenger : "")}");
            }
        }

        public List<Flight>? LoadFlights(string path)
        {
            var flights = new List<Flight>();
            try
            {
                using StreamReader sr = new StreamReader(path);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string?[] data = line.Split(", ");
                    if (data.Length < 9 || data.Length > 10)
                    {
                        Console.WriteLine("Invalid data format in line: " + line);
                        continue;
                    }

                    if (Enum.TryParse(data[7], true, out FlightClass ticketClass) &&
                        DateTime.TryParse(data[4], out DateTime departureDate))
                    {
                        flights.Add(new Flight(data[0], data[1], data[2], data[3], departureDate, data[5], data[6],
                            ticketClass,
                            data.Length == 10 ? data[9] : "", data[8] == "True"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return flights;
        }
    }

    public class BookingManager : IBookingManager
    {
        private readonly List<Flight>? _flights;

        public BookingManager(List<Flight>? flights)
        {
            _flights = flights;
        }

        public bool BookFlight(User user, string flightId)
        {
            if (user == null) return false;
            var flight = _flights.FirstOrDefault(f => f.Id == flightId && !f.Booked);
            if (flight == null)
            {
                Console.WriteLine("Flight not available for booking.");
                return false;
            }

            flight.Passenger = user.Username;
            flight.Booked = true;
            Console.WriteLine("Flight booked successfully.");
            return true;
        }

        public bool CancelBooking(User user, string flightId)
        {
            var flight = _flights.FirstOrDefault(f => f.Id == flightId && f.Passenger == user.Username);
            if (flight == null)
            {
                Console.WriteLine("Booking not found.");
                return false;
            }

            flight.Booked = false;
            flight.Passenger = null;
            Console.WriteLine("Booking canceled successfully.");
            return true;
        }

        public string? ModifyBookingClass(User user, string flightId, string classChoice)
        {
            var flight = _flights.FirstOrDefault(f => f.Id == flightId && f.Passenger == user.Username);
            if (flight == null)
            {
                Console.WriteLine("Booking not found.");
                return null;
            }

            Console.WriteLine("Flight found. Current Ticket Class: " + flight.TicketClass);

            if (!string.IsNullOrEmpty(classChoice))
            {
                return UpdateFlightClass(flight, classChoice);
            }
            else
            {
                Console.WriteLine("No changes were made.");
            }
            return null;
        }

      

        private string? UpdateFlightClass(Flight flight, string classChoice)
        {
            FlightClass currentClass = flight.TicketClass;
            string? updatedPrice = flight.Price;
            if (flight.Price == null) return null;

            switch (classChoice)
            {
                case "1":
                    if (currentClass == FlightClass.Business)
                    {
                        updatedPrice = currentClass.FromBusinessToEco(flight.Price);
                    }
                    else if (currentClass == FlightClass.First)
                    {
                        updatedPrice = currentClass.FromFirstToEco(flight.Price);
                    }

                    flight.TicketClass = FlightClass.Economy;
                    break;

                case "2":
                    if (currentClass == FlightClass.Economy)
                    {
                        updatedPrice = currentClass.FromEcoToBusiness(flight.Price);
                    }
                    else if (currentClass == FlightClass.First)
                    {
                        updatedPrice = currentClass.FromFirstToBusiness(flight.Price);
                    }

                    flight.TicketClass = FlightClass.Business;
                    break;

                case "3":
                    if (currentClass == FlightClass.Economy)
                    {
                        updatedPrice = currentClass.FromEcoToFirst(flight.Price);
                    }
                    else if (currentClass == FlightClass.Business)
                    {
                        updatedPrice = currentClass.FromBusinessToFirst(flight.Price);
                    }

                    flight.TicketClass = FlightClass.First;
                    break;

                default:
                    Console.WriteLine("Invalid class selection.");
                    return "-1";
            }

            flight.Price = updatedPrice;
            Console.WriteLine(
                $"Flight class changed successfully. New Class: {flight.TicketClass}, New Price: {flight.Price}");
            return flight.Price;
        }


    }

    public class FlightDisplay : IFlightDisplay
    {
        private readonly List<Flight>? _flights;

        public FlightDisplay(List<Flight>? flights)
        {
            _flights = flights;
        }

        public void ShowFlights(List<Flight>? flights = null)
        {
            flights ??= _flights;
            if (flights == null)
                return;
            foreach (var flight in flights)
            {
                Console.WriteLine(
                    $"ID: {flight.Id}, Price: {flight.Price}, Departure: {flight.DepartureCountry}, Destination: {flight.DestinationCountry}, Date: {flight.DepartureDate}, Status: {(flight.Booked ? "Booked" : "Available")}");
                Console.WriteLine("------------------------------------------------");
            }
        }

        public bool ShowUserBookings(string? user)
        {
            if (user == "") return false;
            var userFlights = _flights.Where(f => f.Passenger.Trim() == user).ToList();
            if (userFlights.Count == 0)
            {
                Console.WriteLine("No bookings found for this user.");
                return false;
            }

            ShowFlights(userFlights);
            return true;
        }
    }

    public class FlightSearch : IFlightSearch
    {
        private readonly List<Flight>? _flights;

        public FlightSearch(List<Flight>? flights)
        {
            _flights = flights;
        }
        public List<Flight>? SearchFlights(string searchTerm)
        {
            DateTime parsedDate;
            bool isDate = DateTime.TryParse(searchTerm, out parsedDate);
            FlightClass parsedClass;
            bool isClass = Enum.TryParse(searchTerm, true, out parsedClass);
            List<Flight>? flights = (from flight in _flights
                where (isDate && (flight.DepartureDate.CompareTo(parsedDate) == 0)
                       || flight.Price == searchTerm
                       || flight.DepartureCountry == searchTerm
                       || flight.DestinationCountry == searchTerm
                       || flight.DepartureAirport == searchTerm
                       || flight.ArrivalAirport == searchTerm
                       || flight.Id == searchTerm
                       || (isClass && flight.TicketClass == parsedClass))
                      && flight.Booked == false
                select flight).ToList();
            if (flights.Count == 0) Console.WriteLine("No flights found.");
            return flights;
        }

        public List<Flight> FilterBookings(string searchTerm)
        {
            DateTime parsedDate;
            bool isDate = DateTime.TryParse(searchTerm, out parsedDate);
            FlightClass parsedClass;
            bool isClass = Enum.TryParse(searchTerm, true, out parsedClass);
            List<Flight> flights = (from flight in _flights
                where (isDate && (flight.DepartureDate.CompareTo(parsedDate) == 0)
                       || flight.Price == searchTerm
                       || flight.DestinationCountry == searchTerm
                       || flight.DepartureAirport == searchTerm
                       || flight.ArrivalAirport == searchTerm
                       || flight.Id == searchTerm
                       || flight.Passenger == searchTerm
                       || (isClass && flight.TicketClass == parsedClass))
                select flight).ToList();
            if (flights.Count == 0) Console.WriteLine("No flights found.");

            return flights;
        }
    }
        #endregion

    #region FlightList Management Class
    public class FlightList
    {
        private static FlightList? _instance;
        private readonly static object LockObject = new object();
        private readonly List<Flight>? _flights;
        private readonly IFlightPersistence _persistence;
        private readonly IBookingManager _bookingManager;
        private readonly IFlightDisplay _display;
        private readonly IFlightSearch _search;
        private FlightList(string filePath)
        {
            _persistence = new FlightPersistence();
            _flights = _persistence.LoadFlights(filePath);
            _bookingManager = new BookingManager(_flights);
            _display = new FlightDisplay(_flights);
            _search = new FlightSearch(_flights);
        }

        public static FlightList GetInstance(string filePath)
        {
            lock (LockObject)
            {
                _instance ??= new FlightList(filePath);
            }
            return _instance;
        }

        public void SaveFlightsToFile(string path) => _persistence.SaveFlights(_flights, path);
        public void DisplayAllFlights() => _display.ShowFlights(_flights);
        public bool DisplayUserBookings(string? user) => _display.ShowUserBookings(user);
        public bool BookFlight(User user, string flightId) => _bookingManager.BookFlight(user, flightId);
        public bool CancelBooking(User user, string flightId) => _bookingManager.CancelBooking(user, flightId);

        public string? ModifyBooking(User user, string flightId, string flightClass) =>
            _bookingManager.ModifyBookingClass(user, flightId,flightClass);

        public void SearchFlights(string searchTerm)
        {
            List<Flight>? flights = _search.SearchFlights(searchTerm);
            _display.ShowFlights(flights);
        }
        public void FilterBookings(string? searchTerm)
        {

            if (searchTerm != null)
            {
                List<Flight> flights = _search.FilterBookings(searchTerm);
            }
            if (searchTerm != null)
                _display.ShowUserBookings(searchTerm);

        }

    }
        #endregion

}