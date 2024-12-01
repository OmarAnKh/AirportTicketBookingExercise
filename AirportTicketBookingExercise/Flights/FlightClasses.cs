using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AirportTicketBookingExercise.Flight
{
    public enum FlightClass{
        First,
        Business,
        Economy
    }

    public static class FlightClassEXT
    {
        public static string? FromEcoToBusiness(this FlightClass flight, string price)
        {
            float priceInInt = FromStringToInt(price);
            priceInInt = priceInInt * 2f;
            return $"{priceInInt}$";
        }

        public static string? FromEcoToFirst(this FlightClass flight,string price)
        {
            float priceInInt =FromStringToInt(price);
            priceInInt = priceInInt * 3f;
            return $"{priceInInt}$";
        }


        public static string? FromBusinessToEco(this FlightClass flight, string price)
        {
            float priceInInt = FromStringToInt(price);
            priceInInt = priceInInt / 2f;
            return $"{priceInInt:0}$";
        }
        public static string? FromBusinessToFirst(this FlightClass flight, string price)
        {
            float priceInInt = FromStringToInt(price);
            priceInInt = priceInInt *1.5f;
            return $"{priceInInt}$";
        }


        public static string? FromFirstToEco(this FlightClass flight, string price)
        {
            float priceInInt = FromStringToInt(price);
            priceInInt = priceInInt / 3f;
            return $"{priceInInt:0}$";
        }


        public static string? FromFirstToBusiness(this FlightClass flight, string price)
        {
            float priceInInt = FromStringToInt(price);
            priceInInt = priceInInt / 2f;
            return $"{priceInInt:0}$";
        }


        public static float FromStringToInt(string price) {
            string[] number = price.Split('$');
            if (number.Length > 1 && int.TryParse(number[0], out int result))
            {
                return result;
            }
            Console.WriteLine("could'nt convert");
            System.Environment.Exit(1);
            return 0;

        }

    }


}
