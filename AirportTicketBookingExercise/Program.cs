using AirportTicketBookingExercise.Flight;
using AirportTicketBookingExercise.Users;

namespace AirportTicketBookingExercise
{
    static class Program
    {
        private static void Main()
        {
            UserList? userList = UserList.GetInstance("../../../files/Users.txt");
            FlightList flightList = FlightList.GetInstance("../../../files/Flights.txt");


            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Welcome to the Airport Ticket Booking System");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Please choose an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Register(userList);
                        break;
                    case "2":
                        if (Login(userList))
                        {
                            ShowMainMenu(userList, flightList);
                        }
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            userList.SaveUsersToFile("../../../files/Users.txt");
            flightList.SaveFlightsToFile("../../../files/Flights.txt");
            Console.WriteLine("Goodbye!");
        }


        #region Register
        private static void Register(UserList? userList)
        {
            Console.WriteLine("Register a new account:");
            Console.Write("Username: ");
            string? username = Console.ReadLine();
            Console.Write("Password: ");
            string? password = Console.ReadLine();

            Console.WriteLine(userList.AddNewUser(username, password) ? "Registration successful." : "Username already exists. Try again.");
        }
        #endregion

        #region Login
        private static bool Login(UserList? userList)
        {
            Console.WriteLine("Login to your account:");
            Console.Write("Username: ");
            string? username = Console.ReadLine();
            Console.Write("Password: ");
            string? password = Console.ReadLine();

            User? user = userList.CheckCredentials(username, password);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials. Try again.");
                return false;
            }
            Console.WriteLine($"Login successful. Welcome {user.Username}!");
            return true;
        }
        #endregion

        #region ShowMainMenu
        private static void ShowMainMenu(UserList? userList, FlightList flightList)
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Book a Flight");
                Console.WriteLine("2. Search for a Flight");
                Console.WriteLine("3. Cancel a Booking");
                Console.WriteLine("4. Modify a Booking");
                Console.WriteLine("5. Show My Bookings");
                Console.WriteLine("6. Show Flights");
                Console.WriteLine("7. Filter Bookings");
                Console.WriteLine("8. Logout");
                Console.Write("Please choose an option: ");

                string? choice = Console.ReadLine();
                User currentUser = userList.User;
                string? flightId;
                if (currentUser != null)
                    switch (choice)
                    {
                        case "1":
                            flightId = Console.ReadLine();
                            if (flightId != null)
                                flightList.BookFlight(currentUser, flightId);
                            break;
                        case "2":
                            string? seachTerm = Console.ReadLine();
                            if (seachTerm != null)
                                flightList.SearchFlights(seachTerm);
                            break;
                        case "3":
                            flightId = Console.ReadLine();
                            if (flightId != null)
                                flightList.CancelBooking(currentUser, flightId);
                            break;
                        case "4":
                            flightId = Console.ReadLine();
                            if (flightId != null){
                                string? flightClass = GetUserInputForClassChange();
                                flightList.ModifyBooking(currentUser, flightId, flightClass);
                            }
                            break;
                        case "5":
                            flightList.DisplayUserBookings(currentUser.Username);
                            break;
                        case "6":
                            flightList.DisplayAllFlights();
                            break;
                        case "7":
                            if (currentUser.Type == "Manager")
                            {
                                Console.WriteLine("Search for: ");
                                string? searchFor = Console.ReadLine();
                                if (searchFor != null)
                                    flightList.FilterBookings(searchFor);
                            }
                            else
                            {
                                Console.WriteLine("You do not have permission to filter bookings.");
                            }
                            break;
                        case "8":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
            }
        }
        public static string? GetUserInputForClassChange()
        {
            Console.WriteLine("Do you want to change the flight class? (yes/no)");
            string? response = Console.ReadLine();

            if (response?.ToLower() == "yes")
            {
                Console.WriteLine("Choose a new class (1. Economy, 2. Business, 3. First) please choose a number:");
                return Console.ReadLine();
            }

            return null;
        }
        #endregion

    }
}