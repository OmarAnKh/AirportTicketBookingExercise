using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingExercise.Flight
{
    public class Flight
    {
        public string? Id { get; init; }
        public string? Price { get;  set; }
        public string? DepartureCountry { get; private set; }
        public string? DestinationCountry { get; private set; }
        public DateTime DepartureDate { get; private set; }
        public string? DepartureAirport { get; private set; }
        public string? ArrivalAirport { get; private set; }
        public FlightClass TicketClass  { get; set; }
        public bool Booked { get;  set; }
        public string? Passenger { get; set; }

        public Flight(string? Id,string? Price, string? DepartureCountry, string? DestinationCountry, DateTime DepartureDate, string? DepartureAirport, string? ArrivalAirport, FlightClass TicketClass,string? Passenger ,bool Booked=false )
        {
            this.Id = Id;
            this.Price = Price;
            this.DepartureCountry= DepartureCountry;
            this.DestinationCountry = DestinationCountry;
            if (DepartureDate < DateTime.Today)
            {
                this.DepartureDate = DateTime.Today;
            }
            else
            {
                this.DepartureDate = DepartureDate;
            }
            this.DepartureAirport = DepartureAirport;
            this.ArrivalAirport= ArrivalAirport;
            this.TicketClass = TicketClass;
            this.Booked = Booked;
            this.Passenger = Passenger;
        }
    }
}