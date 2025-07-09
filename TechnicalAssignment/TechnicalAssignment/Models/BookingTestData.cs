using System.Collections.Generic;

namespace TechnicalAssignment.Models;

public static class BookingTestData
{
    public record GuestInfo(string FirstName, string LastName, string Email, string Phone);
    
    public record BookingDates(string CheckIn, string CheckOut, int StayDuration);
    
    public record RoomInfo(string RoomType, int ExpectedGuestCount, decimal ExpectedPrice);

    public static GuestInfo ValidGuest => new(
        FirstName: "John",
        LastName: "Smith", 
        Email: "john.smith@example.com",
        Phone: "07123456789"
    );

    public static GuestInfo E2EGuest => new(
        FirstName: "Test",
        LastName: "User",
        Email: "test.user@example.com", 
        Phone: "07987654321"
    );

    public static BookingDates ValidBookingDates => new(
        CheckIn: System.DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"),
        CheckOut: System.DateTime.Now.AddDays(8).ToString("dd/MM/yyyy"),
        StayDuration: 1
    );

    public static BookingDates WeekendBookingDates => new(
        CheckIn: System.DateTime.Now.AddDays(10).ToString("dd/MM/yyyy"),
        CheckOut: System.DateTime.Now.AddDays(12).ToString("dd/MM/yyyy"),
        StayDuration: 2
    );

    public static RoomInfo SingleRoom => new("Single", 1, 100m);
    public static RoomInfo DoubleRoom => new("Double", 2, 150m);
    public static RoomInfo TwinRoom => new("Twin", 2, 150m);
    public static RoomInfo SuiteRoom => new("Suite", 2, 200m);
    public static RoomInfo FamilyRoom => new("Family", 4, 250m);

    public static IEnumerable<RoomInfo> AllRoomTypes => new[]
    {
        SingleRoom, DoubleRoom, TwinRoom, SuiteRoom, FamilyRoom
    };

    public static string InvalidFirstName_Empty = "";
    public static string InvalidFirstName_TooShort = "A";
    public static string InvalidFirstName_WithNumbers = "John123";
    public static string InvalidFirstName_WithSpecialChars = "John@#$";
    public static string InvalidFirstName_Whitespace = "   ";
    public static string InvalidFirstName_TooLong = new string('A', 100);

    public static string InvalidLastName_Empty = "";
    public static string InvalidLastName_TooShort = "B";
    public static string InvalidLastName_WithNumbers = "Smith456";
    public static string InvalidLastName_WithSpecialChars = "Smith!@#";
    public static string InvalidLastName_Whitespace = "   ";
    public static string InvalidLastName_TooLong = new string('B', 100);

    public static string InvalidEmail_NoAtSymbol = "invalid-email";
    public static string InvalidEmail_MissingDomain = "test@";
    public static string InvalidEmail_MissingTLD = "abc@xyz";
    public static string InvalidEmail_Empty = "";

    public static string InvalidPhone_TooShort = "123";
    public static string InvalidPhone_WithLetters = "07abc123456";
    public static string InvalidPhone_NonNumerical = "phone-number";
    public static string InvalidPhone_Empty = "";

    public static IEnumerable<object[]> FieldValidationTestCases
    {
        get
        {
            yield return new object[] { "First Name", InvalidFirstName_TooShort, "ValidLastName", "valid@email.com", "07123456789" };
            yield return new object[] { "First Name", InvalidFirstName_WithNumbers, "ValidLastName", "valid@email.com", "07123456789" };
            yield return new object[] { "First Name", InvalidFirstName_WithSpecialChars, "ValidLastName", "valid@email.com", "07123456789" };
            
            yield return new object[] { "Last Name", "ValidFirstName", InvalidLastName_TooShort, "valid@email.com", "07123456789" };
            yield return new object[] { "Last Name", "ValidFirstName", InvalidLastName_WithNumbers, "valid@email.com", "07123456789" };
            yield return new object[] { "Last Name", "ValidFirstName", InvalidLastName_WithSpecialChars, "valid@email.com", "07123456789" };
            
            yield return new object[] { "Email", "ValidFirstName", "ValidLastName", InvalidEmail_NoAtSymbol, "07123456789" };
            yield return new object[] { "Email", "ValidFirstName", "ValidLastName", InvalidEmail_MissingDomain, "07123456789" };
            yield return new object[] { "Email", "ValidFirstName", "ValidLastName", InvalidEmail_MissingTLD, "07123456789" };
            
            yield return new object[] { "Phone", "ValidFirstName", "ValidLastName", "valid@email.com", InvalidPhone_TooShort };
            yield return new object[] { "Phone", "ValidFirstName", "ValidLastName", "valid@email.com", InvalidPhone_WithLetters };
            yield return new object[] { "Phone", "ValidFirstName", "ValidLastName", "valid@email.com", InvalidPhone_NonNumerical };
        }
    }

    public static IEnumerable<object[]> BoundaryValidationTestCases
    {
        get
        {
            yield return new object[] { "First Name", InvalidFirstName_Empty, "ValidLastName", "valid@email.com", "07123456789" };
            yield return new object[] { "Last Name", "ValidFirstName", InvalidLastName_Empty, "valid@email.com", "07123456789" };
            yield return new object[] { "Email", "ValidFirstName", "ValidLastName", InvalidEmail_Empty, "07123456789" };
            yield return new object[] { "Phone", "ValidFirstName", "ValidLastName", "valid@email.com", InvalidPhone_Empty };
            
            yield return new object[] { "First Name", InvalidFirstName_Whitespace, "ValidLastName", "valid@email.com", "07123456789" };
            yield return new object[] { "Last Name", "ValidFirstName", InvalidLastName_Whitespace, "valid@email.com", "07123456789" };
            
            yield return new object[] { "First Name", InvalidFirstName_TooLong, "ValidLastName", "valid@email.com", "07123456789" };
            yield return new object[] { "Last Name", "ValidFirstName", InvalidLastName_TooLong, "valid@email.com", "07123456789" };
        }
    }

    public static GuestInfo GenerateRandomGuest()
    {
        var random = new System.Random();
        var firstName = $"Test{random.Next(1000, 9999)}";
        var lastName = $"User{random.Next(1000, 9999)}";
        var email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";
        var phone = $"07{random.Next(100000000, 999999999)}";
        
        return new GuestInfo(firstName, lastName, email, phone);
    }

    public static BookingDates GenerateRandomBookingDates(int minDaysFromNow = 1, int maxDaysFromNow = 30, int stayDuration = 1)
    {
        var random = new System.Random();
        var daysFromNow = random.Next(minDaysFromNow, maxDaysFromNow);
        var checkIn = System.DateTime.Now.AddDays(daysFromNow).ToString("dd/MM/yyyy");
        var checkOut = System.DateTime.Now.AddDays(daysFromNow + stayDuration).ToString("dd/MM/yyyy");
        
        return new BookingDates(checkIn, checkOut, stayDuration);
    }
} 