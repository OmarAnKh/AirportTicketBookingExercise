using System.Security.Cryptography;
using System.Text;
using AirportTicketBookingExercise.Flight;

namespace AirportTicketBookingExercise.Users
{
    #region Interfaces
    public interface IUserPersistence
    {
        void SaveUsers(List<User> users, string path, int count);
        (List<User> Users, int count) LoadUsers(string path);
    }

    public interface IUserManager
    {
        public bool AddNewUser(string username, string password);
        public User? CheckCredentials(string? username, string? password);
        private static byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            throw new NotImplementedException();
        }
        private static string Decrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            throw new NotImplementedException();
        }
    }

    public interface IUserDisplay
    {
        public void DisplayUsers(List<User> users);
    }
    #endregion


    #region Implementations
    public class UserPersistence : IUserPersistence
    {
        public void SaveUsers(List<User> users, string path, int count)
        {
            StreamWriter sw = new StreamWriter(path, true, Encoding.ASCII);
            for (int i = count; i < users.Count; i++)
            {
                User user = users[i];
                sw.WriteLine($"{user.Username}, {user.Password}, {user.Type}, {user.Key}, {user.IV} ");

            }
            sw.Close();
        }
        public (List<User> Users, int count) LoadUsers(string path)
        {
            List<User> users = [];
            int count = 0;
            try
            {
                StreamReader sr = new StreamReader(path);
                string? line = sr.ReadLine();
                while (line != null)
                {

                    string[] userInfo = line.Split(", ");
                    users.Add(new User(userInfo[0], userInfo[1], userInfo[2], userInfo[3], userInfo[4]));
                    line = sr.ReadLine();
                }
                sr.Close();
                count = users.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't Read from the file: {ex.Message}");
                System.Environment.Exit(1);
            }
            return (users, count);
        }
    }


    public class UserManager : IUserManager
    {
        private readonly List<User> _users;
        private User _user;
        public UserManager(List<User> users, User user)
        {
            _users = users;
            _user = user;
        }
        public bool AddNewUser(string username, string password)
        {
            if (username == string.Empty || password == string.Empty) return false;
            
            User? user = (from one in _users where one.Username == username select one).FirstOrDefault();
           
            if (user != null) return false;
            
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }

            byte[] encryptedPassword = Encrypt(password, key, iv);
            string encryptedPasswordString = Convert.ToBase64String(encryptedPassword);
            string keyString = Convert.ToBase64String(key);
            string ivString = Convert.ToBase64String(iv);

            _users.Add(new User(username, encryptedPasswordString, "Passenger", keyString, ivString));
            return true;
        }


        public User? CheckCredentials(string? username, string? password)
        {
            User? user = (from one in _users where one.Username == username select one).FirstOrDefault();
            if (user == null) return user;


            byte[] encryptedPassword = Convert.FromBase64String(user.Password);
            byte[] key = Convert.FromBase64String(user.Key);
            byte[] iv = Convert.FromBase64String(user.IV);

            string decryptedPassword = Decrypt(encryptedPassword, key, iv);

            if (decryptedPassword == password)
            {
                _user = new User(user.Username, user.Password, user.Type, user.Key, user.IV);
                return _user;
            }


            return null;
        }


        private static byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            return msEncrypt.ToArray();
        }

        public static string Decrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherText);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

    }

    public class UserDisplay : IUserDisplay
    {
        public void DisplayUsers(List<User> users)
        {
            foreach (var user in users)
            {
                Console.WriteLine($"Username: {user.Username}, Type: {user.Type}");
            }
        }
    }
    #endregion

    #region UserList Managment Class
    public class UserList
    {
        private static UserList? _instance;
        private readonly static object LockObject = new object();

        private readonly List<User> _users;
        public User? User { get; private set; }
        private readonly int _count;
        private readonly IUserPersistence _userPersistence;
        private readonly IUserManager _userManager;
        private readonly IUserDisplay _userDisplay;

        private UserList(string filePath)
        {
            _userPersistence = new UserPersistence();
            (List<User> Users, int count) usersInfo = _userPersistence.LoadUsers(filePath);
            _users = usersInfo.Users;
            _count = usersInfo.count;
            _userManager = new UserManager(_users, User);
            _userDisplay = new UserDisplay();
        }

        public static UserList? GetInstance(string filePath)
        {
            lock (LockObject)
            {
                _instance ??= new UserList(filePath);
            }
            return _instance;
        }

        public void SaveUsersToFile(string path) => _userPersistence.SaveUsers(_users, path, _count);
        public void DisplayUsers(List<User> users) => _userDisplay.DisplayUsers(users);
        public bool AddNewUser(string username, string password) => _userManager.AddNewUser(username, password);
        public User? CheckCredentials(string? username, string? password)
        {
            User? user = _userManager.CheckCredentials(username, password);
            User = user;
            return user;
        }


    }
    #endregion


}