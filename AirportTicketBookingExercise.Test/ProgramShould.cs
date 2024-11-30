using AirportTicketBookingExercise.Flight;
using AirportTicketBookingExercise.Users;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AirportTicketBookingExercise.Test
{
    public class ProgramShould
    {
        private readonly UserList? _users;
        private readonly FlightList _flights;
        private readonly User _user;


        public ProgramShould()
        {
            var fixture = new Fixture();
            _users = UserList.GetInstance("../../../../AirportTicketBookingExercise/files/Users.txt");
            _flights = FlightList.GetInstance("../../../../AirportTicketBookingExercise/files/Flights.txt");
            _user = fixture.Create<User>();

        }
        [Theory]
        [InlineData("Omar Khalili", "Khalili@1234", false)]
        [InlineData("Omar Khalili", "", false)]
        [InlineData("", "Khalili@1234", false)]
        [InlineData("Ahmad Khalili", "Ahmad@1234", true)]
        public void CheckRegister(string username, string password, bool expectedResult)
        {
            //Arrange

            //Act
            bool addingNewUserResult = _users.AddNewUser(username, password);

            //Assert
            Assert.Equal(addingNewUserResult, expectedResult);
        }


        [Theory]
        [InlineData("Omar Khalili", "Khalili@1234", true)]
        [InlineData("Omar Khalili", "", false)]
        [InlineData("", "Khalili@1234", false)]
        [InlineData("Ahmad Khalili", "Ahmad@1234", false)]
        public void CheckLogin(string username, string password, bool expectedResult)
        {
            //arrange
            User? user = _users.CheckCredentials(username, password);

            //act
            bool result = user != null;

            //Assert
            Assert.Equal(result, expectedResult);
        }


        [Theory]
        [InlineData("2", false)]
        [InlineData("4", true)]
        [InlineData("", false)]
        public void CheckFlightBooking(string flightId, bool expectedResult)
        {
            //arrange

            //act
            var result = _flights.BookFlight(_user, flightId);

            //assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Omar Khalili", "2", false)]
        [InlineData("Abood Jbr", "2", true)]
        [InlineData("", "2", false)]
        [InlineData("Omar Khalili", "", false)]
        public void CancelFlightBooking(string username, string flightId, bool expectedResult)
        {
            //arrange
            var autoFixture = new Fixture();
            var user = autoFixture.Build<User>()
                .With(x => x.Username, username)
                .Create();

            //act
            var result = _flights.CancelBooking(user, flightId);

            //assert
            result.Should().Be(expectedResult);
        }


        [Theory]
        [InlineData("Omar Khalili", true)]
        [InlineData("", false)]
        public void DisplayUserBooking(string username, bool expectedResult)
        {
            //arrange

            //act
            var result = _flights.DisplayUserBookings(username);

            //assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("Abood Jbr","2", "3", "900$")]
        [InlineData("Ahmad Iyrot ","8", "2", "640$")]
        [InlineData("Samira","3", "3", "675$")]
        [InlineData("Oliver","7", "1", "300$")]
        [InlineData("Amjad Awad","10", "2", "245$")]
        [InlineData("Abood Jbr","5", "1", "183$")]
        [InlineData("","5", "1", null)]
        [InlineData("Abood Jbr","", "1", null)]
        [InlineData("Omar Khalili","6", "1", "380$")]
        public void ModifyUserBooking(string username ,string flightId, string flightClass, string expectedResult)
        {
            //arrange
            var autoFixture = new Fixture();
            var user= autoFixture.Build<User>().With(x=>x.Username, username).Create();
            
            //act
            var result=_flights.ModifyBooking(user,flightId,flightClass);
            
            //assert
            
            result.Should().Be(expectedResult);
            
        }

    }
}