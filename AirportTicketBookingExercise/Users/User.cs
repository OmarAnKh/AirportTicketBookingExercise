using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingExercise.Users
{
    public record User(string Username, string Password, string Type, string Key, string IV)
    {

    }
}