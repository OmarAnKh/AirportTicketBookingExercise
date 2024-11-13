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

            if (username != null && password != null && userList.AddNewUser(username, password))
            {
                Console.WriteLine("Registration successful.");
            }
            else
            {
                Console.WriteLine("Username already exists. Try again.");
            }
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
                            flightList.CancelBooking(currentUser.Username, flightId);
                        break;
                    case "4":
                        flightId = Console.ReadLine();
                        if (flightId != null)
                            flightList.ModifyBooking(currentUser, flightId);
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
        #endregion


        #region GetDataFromFile
        public static Dictionary<string, List<string>> GetDataFromFile(string FilePath)
        {
            List<string>? DataFromAFile = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(FilePath);
                string? line = sr.ReadLine();
                while (line != null)
                {
                    DataFromAFile.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
                if (DataFromAFile.Count == 0)
                {
                    DataFromAFile = null;
                }
                return ListToDictionary(DataFromAFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt Read from the file: {ex.Message}");
                DataFromAFile = null;
                return ListToDictionary(DataFromAFile);
            }
        }
        #endregion

        #region AddNewUser
        public static void AddNewUser(ref Dictionary<string, List<string>> Users)
        {
            Console.WriteLine("Please enter the Username (it must be uniqe):");
            string? user = Console.ReadLine();

            while (user == null || Users.ContainsKey(user))
            {
                Console.WriteLine("Please Enter a valid Username(it must be uniqe):");
                user = Console.ReadLine();
                Console.WriteLine(user);
            }
            Console.WriteLine("Please enter Your Password ");
            string? password = Console.ReadLine();
            while (password == null)
            {
                Console.WriteLine("You must enter a password");
                password = Console.ReadLine();
            }
            user = $"{user}, {password}, Passenger";
            Users.Add(user, new List<string> { $",{password}", ", Passenger" });

        }
        #endregion

        #region WriteDataToFile
        public static void WriteDataToFile(string FilePath, List<string> Data)
        {
            try
            {
                StreamWriter sw = new StreamWriter(FilePath);
                foreach (string line in Data)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could'nt write on the file: {ex.Message}");
            }
        }
        #endregion

        #region CheckCredentials
        public static List<string> CheckCredentials(Dictionary<string, List<string>> Users)
        {
            int chances = 3;


            Console.WriteLine("What is your user name: ");
            string? username = Console.ReadLine();
            Console.WriteLine("What is your password: ");
            string? password = Console.ReadLine();
            while (username == null)
            {
                Console.WriteLine("Please Enter a username: ");
                username = Console.ReadLine();
            }

            while (chances > 0)
            {
                if (!Users.ContainsKey(username))
                {
                    Console.WriteLine($"One or more of your credentials are wrong (you have {chances - 1} chances left).");
                    chances--;
                    Console.WriteLine("Enter username: ");
                    username = Console.ReadLine();
                    Console.WriteLine("Enter password: ");
                    password = Console.ReadLine();

                    while (username == null)
                    {
                        Console.WriteLine("Please Enter a username: ");
                        username = Console.ReadLine();
                    }

                    continue;
                }

                List<string> userInfo = Users[username];

                if (userInfo.ElementAt(1) == password)
                {
                    return userInfo;
                }
                Console.WriteLine($"One or more of your credentials are wrong (you have {chances - 1} chances left).");

                chances--;

                if (chances > 0)
                {
                    Console.WriteLine("Enter username: ");
                    username = Console.ReadLine();
                    Console.WriteLine("Enter password: ");
                    password = Console.ReadLine();

                    while (username == null)
                    {
                        Console.WriteLine("Please Enter a username: ");
                        username = Console.ReadLine();
                    }
                }
            }

            Console.WriteLine("Too many failed attempts. Exiting.");
            Environment.Exit(1);
            return new List<string>();
        }
        #endregion

        #region ListToDictionary
        public static Dictionary<string, List<string>> ListToDictionary(List<string> Data)
        {
            Dictionary<string, List<string>> DictionaryData = new Dictionary<string, List<string>>();
            if (Data == null)
            {
                return DictionaryData;
            }
            foreach (string line in Data)
            {
                string[] statements = line.Split(", ");
                DictionaryData.Add(statements[0], statements.ToList());
            }
            return DictionaryData;

        }
        #endregion
    }
}