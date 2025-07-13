using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Contact")]
[Category("HomePage")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageContactTests : BaseTest
{
    private HomePage _homePage = null!;

    public static object[] MobileViewports => ViewportTestData.GetMobileViewportTestCases();
    public static object[] TabletViewports => ViewportTestData.GetTabletViewportTestCases();
    public static object[] DesktopViewports => ViewportTestData.GetDesktopViewportTestCases();

    [SetUp]
    public void HomePageSetUp()
    {
        Logger.LogInformation("Starting HomePage Contact test setup");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        Logger.LogDebug("Navigated to URL: {Url}", TestConfig.BaseUrl);

        _homePage = GetService<HomePage>();
        _homePage.WaitForPageToLoad();
        Logger.LogDebug("HomePage loaded successfully for Contact tests");
    }

    [Test]
    [Description("TC001: Verify complete contact form submission with valid data")]
    public void ContactForm_WithValidData_ShouldSubmitSuccessfully()
    {
        Logger.LogInformation("Starting TC001: Contact form submission test with valid data");
        
        Logger.LogDebug("Scrolling to contact form section");
        _homePage.Contact.ScrollToContactForm();
        
        Logger.LogDebug("Verifying contact form is visible");
        Assert.That(_homePage.Contact.IsContactFormVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Contact form should be visible after scrolling");
        
        Logger.LogDebug("Filling contact form with valid data");
        var validData = ContactFormTestData.ValidContact;
        _homePage.Contact.FillContactForm(
            name: validData.Name,
            email: validData.Email, 
            phone: validData.Phone,
            subject: validData.Subject,
            message: validData.Message
        );
        
        Logger.LogDebug("Submitting contact form");
        _homePage.Contact.SubmitContactForm();
        
        Logger.LogDebug("Waiting for success message to appear after form submission");
        Assert.That(_homePage.Contact.WaitForContactSubmissionSuccess(TimeSpan.FromSeconds(10)), Is.True, 
            "Contact form submission success message should appear after form submission");
        
        Logger.LogInformation("TC001: Contact form submission test with valid data passed successfully");
    }

    [TestCaseSource(typeof(ContactFormTestData), nameof(ContactFormTestData.ValidationTestCases))]
    [Description("TC002: Verify contact form validation for various invalid inputs")]
    public void ContactForm_WithInvalidData_ShouldShowValidationError(string description, ContactFormTestData.ContactInfo testData, string expectedError)
    {
        Logger.LogInformation("Starting validation test: {Description}", description);

        Logger.LogDebug("Scrolling to contact form");
        _homePage.Contact.ScrollToContactForm();

        Logger.LogDebug("Filling contact form with invalid data for '{Description}'", description);
        _homePage.Contact.FillContactForm(
            name: testData.Name,
            email: testData.Email,
            phone: testData.Phone,
            subject: testData.Subject,
            message: testData.Message
        );

        Logger.LogDebug("Submitting contact form");
        _homePage.Contact.SubmitContactForm();

        Logger.LogDebug("Waiting for validation errors to appear");
        Assert.That(_homePage.Contact.AreContactValidationErrorsVisible(TimeSpan.FromSeconds(5)), Is.True,
            "Validation errors should appear for invalid data");

        Logger.LogDebug("Verifying validation error message");
        var validationErrors = _homePage.Contact.GetContactValidationErrors();
        Assert.That(validationErrors, Does.Contain(expectedError),
            $"Should show validation error '{expectedError}' for {description}. Found errors: {string.Join(", ", validationErrors)}");

        Logger.LogInformation("Validation test '{Description}' passed successfully", description);
    }

    [Test]
    [Description("TC003: Verify multiple validation errors display simultaneously")]
    public void ContactForm_WithAllFieldsEmpty_ShouldShowAllValidationErrors()
    {
        Logger.LogInformation("Starting TC003: Multiple field validation errors test");
        
        Logger.LogDebug("Scrolling to contact form");
        _homePage.Contact.ScrollToContactForm();
        
        Logger.LogDebug("Leaving all contact form fields empty and submitting");
        _homePage.Contact.FillContactForm("", "", "", "", "");
        
        Logger.LogDebug("Submitting contact form with all empty fields");
        _homePage.Contact.SubmitContactForm();
        
        Logger.LogDebug("Waiting for validation errors to appear");
        Assert.That(_homePage.Contact.AreContactValidationErrorsVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Validation errors should appear when all fields are empty");
        
        Logger.LogDebug("Getting all validation error messages");
        var validationErrors = _homePage.Contact.GetContactValidationErrors();
        
        Logger.LogDebug("Found {ErrorCount} validation errors: {Errors}", 
            validationErrors.Count, string.Join(", ", validationErrors));
        
        Logger.LogDebug("Verifying all expected validation errors are present");
        Assert.Multiple(() =>
        {
            Assert.That(validationErrors, Does.Contain("Name may not be blank"), 
                "Should show 'Name may not be blank' validation error");
            
            Assert.That(validationErrors, Does.Contain("Email may not be blank"), 
                "Should show 'Email may not be blank' validation error");
            
            Assert.That(validationErrors, Does.Contain("Phone may not be blank"), 
                "Should show 'Phone may not be blank' validation error");
            
            Assert.That(validationErrors, Does.Contain("Subject may not be blank"), 
                "Should show 'Subject may not be blank' validation error");
            
            Assert.That(validationErrors, Does.Contain("Message may not be blank"), 
                "Should show 'Message may not be blank' validation error");
        });
        
        Logger.LogDebug("Verifying minimum number of validation errors (at least 5 for required fields)");
        Assert.That(validationErrors.Count, Is.GreaterThanOrEqualTo(5), 
            $"Should have at least 5 validation errors for empty required fields. Found {validationErrors.Count} errors");
        
        Logger.LogInformation("TC003: Multiple field validation errors test passed successfully");
    }

    [Test]
    [Description("TC004: Verify complete message lifecycle from submission to admin panel deletion")]
    public void ContactForm_EndToEndFlow_ShouldCreateDisplayAndDeleteMessage()
    {
        Logger.LogInformation("Starting TC004: End-to-end message lifecycle test");
        
        var e2eData = ContactFormTestData.E2EContact;
        
        Logger.LogDebug("Step 1-4: Submitting contact form with E2E test data");
        _homePage.Contact.ScrollToContactForm();
        _homePage.Contact.FillContactForm(e2eData.Name, e2eData.Email, e2eData.Phone, e2eData.Subject, e2eData.Message);
        _homePage.Contact.SubmitContactForm();
        
        Logger.LogDebug("Step 5: Verifying contact form submission success");
        Assert.That(_homePage.Contact.WaitForContactSubmissionSuccess(TimeSpan.FromSeconds(10)), Is.True, 
            "Contact form submission should be successful");
        
        Logger.LogDebug("Step 6: Navigating to admin panel");
        Driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/admin");
        
        var adminLoginPage = GetService<AdminLoginPage>();
        adminLoginPage.WaitForPageToLoad();
        
        Logger.LogDebug("Step 7: Logging into admin panel");
        Assert.That(adminLoginPage.IsOnLoginPage(), Is.True, 
            "Should be on admin login page");
        
        adminLoginPage.Login(TestConfig.AdminCredentials.Username, TestConfig.AdminCredentials.Password);
        
        var adminNavBar = GetService<AdminNavBarComponent>();
        adminNavBar.WaitForPageToLoad();
        
        Logger.LogDebug("Verifying successful login");
        Assert.That(adminNavBar.IsLoggedIn(), Is.True, 
            "Should be successfully logged into admin panel");
        
        Logger.LogDebug("Step 8: Navigating to Messages section");
        var adminMessagesPage = adminNavBar.NavigateToMessages();
        
        Logger.LogDebug("Step 9-10: Verifying test message appears in messages list");
        Assert.That(adminMessagesPage.IsMessagePresent(e2eData.Name, e2eData.Subject), Is.True, 
            $"Test message should appear in admin panel with name '{e2eData.Name}' and subject '{e2eData.Subject}'");
        
        Logger.LogDebug("Cleanup: Deleting test message from admin panel");
        var deleteSuccess = adminMessagesPage.DeleteMessage(e2eData.Name, e2eData.Subject);
        Assert.That(deleteSuccess, Is.True, 
            "Test message should be successfully deleted from admin panel");
        
        Logger.LogDebug("Verifying message deletion");
        Assert.That(adminMessagesPage.IsMessagePresent(e2eData.Name, e2eData.Subject), Is.False, 
            "Test message should no longer appear in admin panel after deletion");
        
        Logger.LogInformation("TC004: End-to-end message lifecycle test passed successfully");
    }

    [Test]
    [Description("TC005: Verify form fields have proper labels and accessibility attributes")]
    public void ContactForm_FieldAccessibility_ShouldHaveProperLabelsAndAttributes()
    {
        Logger.LogInformation("Starting TC005: Contact form field accessibility test");
        
        Logger.LogDebug("Scrolling to contact form");
        _homePage.Contact.ScrollToContactForm();
        
        Logger.LogDebug("Verifying form field accessibility attributes");
        Assert.Multiple(() =>
        {
            Logger.LogDebug("Checking name field label and attributes");
            var nameField = _homePage.Contact.GetNameField();
            var nameLabel = _homePage.Contact.GetNameLabel();
            Assert.That(nameLabel.Text, Is.EqualTo("Name"), "Name field should have proper label text");
            Assert.That(nameLabel.GetAttribute("for"), Is.EqualTo(nameField.GetAttribute("id")), "Name label 'for' attribute should match field 'id'");
            Assert.That(nameField.GetAttribute("type"), Is.EqualTo("text"), "Name field should have type='text'");
            Assert.That(nameField.GetAttribute("data-testid"), Is.EqualTo("ContactName"), "Name field should have data-testid='ContactName'");
            
            Logger.LogDebug("Checking email field label and attributes");
            var emailField = _homePage.Contact.GetEmailField();
            var emailLabel = _homePage.Contact.GetEmailLabel();
            Assert.That(emailLabel.Text, Is.EqualTo("Email"), "Email field should have proper label text");
            Assert.That(emailLabel.GetAttribute("for"), Is.EqualTo(emailField.GetAttribute("id")), "Email label 'for' attribute should match field 'id'");
            Assert.That(emailField.GetAttribute("type"), Is.EqualTo("email"), "Email field should have type='email'");
            Assert.That(emailField.GetAttribute("data-testid"), Is.EqualTo("ContactEmail"), "Email field should have data-testid='ContactEmail'");
            
            Logger.LogDebug("Checking phone field label and attributes");
            var phoneField = _homePage.Contact.GetPhoneField();
            var phoneLabel = _homePage.Contact.GetPhoneLabel();
            Assert.That(phoneLabel.Text, Is.EqualTo("Phone"), "Phone field should have proper label text");
            Assert.That(phoneLabel.GetAttribute("for"), Is.EqualTo(phoneField.GetAttribute("id")), "Phone label 'for' attribute should match field 'id'");
            Assert.That(phoneField.GetAttribute("type"), Is.EqualTo("tel"), "Phone field should have type='tel'");
            Assert.That(phoneField.GetAttribute("data-testid"), Is.EqualTo("ContactPhone"), "Phone field should have data-testid='ContactPhone'");
            
            Logger.LogDebug("Checking subject field label and attributes");
            var subjectField = _homePage.Contact.GetSubjectField();
            var subjectLabel = _homePage.Contact.GetSubjectLabel();
            Assert.That(subjectLabel.Text, Is.EqualTo("Subject"), "Subject field should have proper label text");
            Assert.That(subjectLabel.GetAttribute("for"), Is.EqualTo(subjectField.GetAttribute("id")), "Subject label 'for' attribute should match field 'id'");
            Assert.That(subjectField.GetAttribute("type"), Is.EqualTo("text"), "Subject field should have type='text'");
            Assert.That(subjectField.GetAttribute("data-testid"), Is.EqualTo("ContactSubject"), "Subject field should have data-testid='ContactSubject'");
            
            Logger.LogDebug("Checking message field label and attributes");
            var messageField = _homePage.Contact.GetMessageField();
            var messageLabel = _homePage.Contact.GetMessageLabel();
            Assert.That(messageLabel.Text, Is.EqualTo("Message"), "Message field should have proper label text");
            Assert.That(messageLabel.GetAttribute("for"), Is.EqualTo(messageField.GetAttribute("id")),
                "Accessibility Fail: The 'for' attribute of a <label> must match the 'id' of its corresponding form field.");
            Assert.That(messageField.TagName.ToLower(), Is.EqualTo("textarea"), "Message field should be a textarea element");
            Assert.That(messageField.GetAttribute("data-testid"), Is.EqualTo("ContactDescription"), "Message field should have data-testid='ContactDescription'");
            Assert.That(messageField.GetAttribute("rows"), Is.EqualTo("5"), "Message field should have rows='5'");
        });
        
        Logger.LogDebug("Verifying all fields have Bootstrap form-control class");
        var allFormFields = _homePage.Contact.GetAllFormFields();
        foreach (var field in allFormFields)
        {
            Assert.That(field.GetAttribute("class"), Does.Contain("form-control"), 
                $"Field {field.GetAttribute("id")} should have Bootstrap form-control class");
        }
        
        Logger.LogDebug("Verifying submit button accessibility");
        var submitButton = _homePage.Contact.GetSubmitButton();
        Assert.That(submitButton.Text, Is.EqualTo("Submit"), "Submit button should have proper text");
        Assert.That(submitButton.GetAttribute("class"), Does.Contain("btn"), "Submit button should have Bootstrap btn class");
        Assert.That(submitButton.GetAttribute("class"), Does.Contain("btn-primary"), "Submit button should have Bootstrap btn-primary class");
        
        Logger.LogInformation("TC005: Contact form field accessibility test passed successfully");
    }

    [TestCaseSource(nameof(MobileViewports))]
    [TestCaseSource(nameof(TabletViewports))]
    [TestCaseSource(nameof(DesktopViewports))]
    [Description("TC006: Verify contact form displays correctly on different viewports")]
    public void ContactForm_ResponsiveBehavior_ShouldMaintainFunctionalityAcrossViewports(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC006: Contact form responsive behavior test on {deviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        Logger.LogDebug("Setting viewport size to {Width}x{Height} for {deviceName}", width, height, deviceName);
        BrowserHelper.SetViewportSize(Driver, width, height);
        
        Logger.LogDebug("Scrolling to contact form on {deviceName}", deviceName);
        _homePage.Contact.ScrollToContactForm();
        
        Logger.LogDebug("Verifying contact form layout and accessibility on {deviceName}", deviceName);
        Assert.Multiple(() =>
        {
            Assert.That(_homePage.Contact.IsContactFormVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Contact form should be visible on {deviceName}");
            
            var nameField = _homePage.Contact.GetNameField();
            var emailField = _homePage.Contact.GetEmailField();
            var phoneField = _homePage.Contact.GetPhoneField();
            var subjectField = _homePage.Contact.GetSubjectField();
            var messageField = _homePage.Contact.GetMessageField();
            var submitButton = _homePage.Contact.GetSubmitButton();
            
            Assert.That(nameField.Displayed, Is.True, 
                $"Name field should be displayed on {deviceName}");
            Assert.That(emailField.Displayed, Is.True, 
                $"Email field should be displayed on {deviceName}");
            Assert.That(phoneField.Displayed, Is.True, 
                $"Phone field should be displayed on {deviceName}");
            Assert.That(subjectField.Displayed, Is.True, 
                $"Subject field should be displayed on {deviceName}");
            Assert.That(messageField.Displayed, Is.True, 
                $"Message field should be displayed on {deviceName}");
            Assert.That(submitButton.Displayed, Is.True, 
                $"Submit button should be displayed on {deviceName}");
            
            Assert.That(nameField.Enabled, Is.True, 
                $"Name field should be enabled on {deviceName}");
            Assert.That(emailField.Enabled, Is.True, 
                $"Email field should be enabled on {deviceName}");
            Assert.That(phoneField.Enabled, Is.True, 
                $"Phone field should be enabled on {deviceName}");
            Assert.That(subjectField.Enabled, Is.True, 
                $"Subject field should be enabled on {deviceName}");
            Assert.That(messageField.Enabled, Is.True, 
                $"Message field should be enabled on {deviceName}");
            Assert.That(submitButton.Enabled, Is.True, 
                $"Submit button should be enabled on {deviceName}");
        });
        
        Logger.LogDebug("Testing form submission functionality on {deviceName}", deviceName);
        var responsiveData = ContactFormTestData.ResponsiveTestContact;
        _homePage.Contact.FillContactForm(responsiveData.Name, responsiveData.Email, responsiveData.Phone, 
            responsiveData.Subject, responsiveData.Message);
        
        _homePage.Contact.SubmitContactForm();
        
        Logger.LogDebug("Verifying form submission success on {deviceName}", deviceName);
        Assert.That(_homePage.Contact.WaitForContactSubmissionSuccess(TimeSpan.FromSeconds(10)), Is.True, 
            $"Contact form submission should work on {deviceName}");
        
        Logger.LogInformation("TC006: Contact form responsive behavior test passed successfully on {deviceName} ({Width}x{Height})", 
            deviceName, width, height);
    }

    [Test]
    [Description("TC007: Verify message modal displays correct details when clicking subject in admin panel")]
    public void ContactForm_MessageModal_ShouldDisplayCorrectDetailsInAdminPanel()
    {
        Logger.LogInformation("Starting TC007: Contact form message modal verification test");
        
        var testData = ContactFormTestData.ModalTestContact;
        
        Logger.LogDebug("Step 1: Submitting contact form with test data");
        _homePage.Contact.ScrollToContactForm();
        
        Assert.That(_homePage.Contact.IsContactFormVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Contact form should be visible");
        
        _homePage.Contact.FillContactForm(testData.Name, testData.Email, testData.Phone, testData.Subject, testData.Message);
        _homePage.Contact.SubmitContactForm();
        
        Assert.That(_homePage.Contact.WaitForContactSubmissionSuccess(TimeSpan.FromSeconds(5)), Is.True, 
            "Contact form submission should succeed");
        
        Logger.LogDebug("Step 2: Navigating to admin panel");
        Driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/admin");
        
        var adminLoginPage = GetService<AdminLoginPage>();
        adminLoginPage.WaitForPageToLoad();
        
        Logger.LogDebug("Step 3: Logging into admin panel");
        adminLoginPage.Login(TestConfig.AdminCredentials.Username, TestConfig.AdminCredentials.Password);
        
        var adminNavBar = GetService<AdminNavBarComponent>();
        adminNavBar.WaitForPageToLoad();
        
        Logger.LogDebug("Step 4: Navigating to messages section");
        var adminMessagesPage = adminNavBar.NavigateToMessages();
        
        Logger.LogDebug("Step 5: Verifying message appears in messages list");
        Assert.That(adminMessagesPage.IsMessagePresent(testData.Name, testData.Subject), Is.True, 
            "Test message should appear in admin messages list");
        
        Logger.LogDebug("Step 6: Clicking on message subject to open modal");
        adminMessagesPage.ClickMessageSubject(testData.Subject);
        
        Logger.LogDebug("Step 7: Waiting for message modal to appear");
        Assert.That(adminMessagesPage.IsMessageModalVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Message modal should be visible after clicking subject");

        Logger.LogDebug("Step 8: Verifying modal content matches submitted data");
        Assert.Multiple(() =>
        {
            Assert.That(adminMessagesPage.GetModalFromName(), Is.EqualTo(testData.Name), 
                "Modal should display correct sender name");
            Assert.That(adminMessagesPage.GetModalEmail(), Is.EqualTo(testData.Email), 
                "Modal should display correct email address");
            Assert.That(adminMessagesPage.GetModalPhone(), Is.EqualTo(testData.Phone), 
                "Modal should display correct phone number");
            Assert.That(adminMessagesPage.GetModalSubject(), Is.EqualTo(testData.Subject), 
                "Modal should display correct subject");
            Assert.That(adminMessagesPage.GetModalMessage(), Is.EqualTo(testData.Message), 
                "Modal should display correct message content");
        });
        
        Logger.LogDebug("Step 9: Closing message modal after verification");
        adminMessagesPage.CloseMessageModal();
        
        Logger.LogDebug("Verifying modal is closed");
        Assert.That(adminMessagesPage.WaitForMessageModalToDisappear(TimeSpan.FromSeconds(2)), Is.True, 
            "Message modal should be closed after clicking close button");
        
        Logger.LogInformation("TC007: Contact form message modal verification test passed successfully");
    }

    [Test]
    [Description("TC008: Verify message modal in admin panel closes correctly")]
    public void ContactForm_MessageModal_ShouldCloseCorrectlyWhenButtonClicked()
    {
        Logger.LogInformation("Starting TC008: Contact form message modal close functionality test");
        
        var testData = ContactFormTestData.ModalCloseTestContact;
        
        _homePage.Contact.ScrollToContactForm();
        _homePage.Contact.FillContactForm(testData.Name, testData.Email, testData.Phone, testData.Subject, testData.Message);
        _homePage.Contact.SubmitContactForm();
        Assert.That(_homePage.Contact.WaitForContactSubmissionSuccess(TimeSpan.FromSeconds(5)), Is.True, 
            "Setup: Contact form submission should succeed");
        
        Driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/admin");
        var adminLoginPage = GetService<AdminLoginPage>();
        adminLoginPage.WaitForPageToLoad();
        adminLoginPage.Login(TestConfig.AdminCredentials.Username, TestConfig.AdminCredentials.Password);
        
        var adminNavBar = GetService<AdminNavBarComponent>();
        adminNavBar.WaitForPageToLoad();
        var adminMessagesPage = adminNavBar.NavigateToMessages();
        
        var messageFound = WaitHelper.WaitForCondition(Driver, d => adminMessagesPage.IsMessagePresent(testData.Name, testData.Subject), TimeSpan.FromSeconds(10));
        Assert.That(messageFound, Is.True, "Setup: Test message should appear in admin messages list");
        
        adminMessagesPage.ClickMessageSubject(testData.Subject);
        Assert.That(adminMessagesPage.IsMessageModalVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Message modal should be visible before testing close");
        
        Logger.LogDebug("Closing message modal");
        adminMessagesPage.CloseMessageModal();
        
        Assert.That(adminMessagesPage.WaitForMessageModalToDisappear(TimeSpan.FromSeconds(2)), Is.True, 
            "Message modal should be closed after clicking close button");
        
        Logger.LogInformation("TC008: Contact form message modal close functionality test passed successfully");
    }
} 