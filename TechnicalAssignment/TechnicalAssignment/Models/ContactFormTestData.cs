using System.Collections.Generic;

namespace TechnicalAssignment.Models;

/// <summary>
/// Provides centralized test data for contact form tests.
/// </summary>
public static class ContactFormTestData
{
    /// <summary>
    /// Represents a complete set of data for the contact form.
    /// </summary>
    public record ContactInfo(string Name, string Email, string Phone, string Subject, string Message);

    /// <summary>
    /// A set of valid data for a standard successful submission.
    /// </summary>
    public static ContactInfo ValidContact => new(
        Name: "John Smith",
        Email: "john.smith@example.com",
        Phone: "12345678901",
        Subject: "Test Inquiry",
        Message: "This is a test message with sufficient length to meet the minimum requirements."
    );

    /// <summary>
    /// Test data for the end-to-end (E2E) message lifecycle test (TC015).
    /// </summary>
    public static ContactInfo E2EContact => new(
        Name: "Test User E2E",
        Email: "test.e2e@example.com",
        Phone: "12345678901",
        Subject: "E2E Test Subject",
        Message: "This is an end-to-end test message to verify the complete flow."
    );
    
    /// <summary>
    /// Test data for the message modal details verification test (TC019).
    /// </summary>
    public static ContactInfo ModalTestContact => new(
        Name: "Modal Test User",
        Email: "modal.test@example.com",
        Phone: "12345678901",
        Subject: "Modal Test Subject",
        Message: "This is a test message for modal verification with sufficient length to meet requirements."
    );

    /// <summary>
    /// Test data for the message modal close functionality test (TC020).
    /// </summary>
    public static ContactInfo ModalCloseTestContact => new(
        Name: "Modal Close Test User",
        Email: "modal.close.test@example.com",
        Phone: "12345678902",
        Subject: "Modal Close Test Subject",
        Message: "This is a test message for modal close functionality."
    );
    
    /// <summary>
    /// Test data for the responsive design test (TC018).
    /// </summary>
    public static ContactInfo ResponsiveTestContact => new(
        Name: "Responsive Test",
        Email: "responsive@test.com",
        Phone: "12345678901",
        Subject: "Responsive Test Subject",
        Message: "This is a responsive test message with sufficient length."
    );

    // Invalid data strings for validation tests
    public static string InvalidEmailFormat = "invalid-email-format";
    public static string ShortPhone = "123456789";
    public static string LongPhone = "1234567890123456789012";
    public static string ShortSubject = "Test";
    public static string LongSubject = new string('A', 101);
    public static string ShortMessage = "Short message";
    public static string LongMessage = new string('A', 2001);
    
    public static IEnumerable<object[]> ValidationTestCases
    {
        get
        {
            yield return new object[] { "Name is empty", ValidContact with { Name = "" }, "Name may not be blank" };
            yield return new object[] { "Email is empty", ValidContact with { Email = "" }, "Email may not be blank" };
            yield return new object[] { "Email is invalid", ValidContact with { Email = InvalidEmailFormat }, "must be a well-formed email address" };
            yield return new object[] { "Phone is empty", ValidContact with { Phone = "" }, "Phone may not be blank" };
            yield return new object[] { "Phone is too short", ValidContact with { Phone = ShortPhone }, "Phone must be between 11 and 21 characters." };
            yield return new object[] { "Phone is too long", ValidContact with { Phone = LongPhone }, "Phone must be between 11 and 21 characters." };
            yield return new object[] { "Subject is empty", ValidContact with { Subject = "" }, "Subject may not be blank" };
            yield return new object[] { "Subject is too short", ValidContact with { Subject = ShortSubject }, "Subject must be between 5 and 100 characters." };
            yield return new object[] { "Subject is too long", ValidContact with { Subject = LongSubject }, "Subject must be between 5 and 100 characters." };
            yield return new object[] { "Message is empty", ValidContact with { Message = "" }, "Message may not be blank" };
            yield return new object[] { "Message is too short", ValidContact with { Message = ShortMessage }, "Message must be between 20 and 2000 characters." };
            yield return new object[] { "Message is too long", ValidContact with { Message = LongMessage }, "Message must be between 20 and 2000 characters." };
        }
    }
} 