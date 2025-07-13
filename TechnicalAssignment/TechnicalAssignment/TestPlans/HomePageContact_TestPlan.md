# Test Plan: Home Page Contact Form

## 1. Overview

This plan covers tests for the contact form on the homepage. It includes submission, validation, accessibility, responsive design, and the end-to-end flow of a message to the admin panel.

## 2. Test Cases

### TC001: Valid Form Submission
- Objective: To verify that the contact form can be submitted successfully with all valid data.
- Steps:
    1. Fill all fields with valid data.
    2. Click 'Submit'.
- Expected Outcome: A success message is displayed.

### TC002: Form Validation for Invalid Inputs
- Objective: To ensure the form displays specific validation errors for various types of invalid data.
- Steps:
    1. For each validation rule (e.g., blank fields, invalid email, short phone number), enter the corresponding invalid data and submit.
- Expected Outcome: A specific error message is displayed for each invalid submission, and the form is not sent.

### TC003: Multiple Validation Errors
- Objective: To verify that multiple validation errors are displayed simultaneously if all fields are empty.
- Steps:
    1. Click 'Submit' without filling in any fields.
- Expected Outcome: An error message is displayed for each of the 5 required fields.

### TC004: End-to-End Message Lifecycle
- Objective: To test the complete flow of a message from submission to deletion in the admin panel.
- Steps:
    1. Submit the contact form.
    2. Log in to the admin panel and navigate to the messages.
    3. Verify the submitted message is present.
    4. Delete the message.
    5. Verify the message is removed from the list.
- Expected Outcome: The message is successfully created, displayed in the admin panel, and deleted.

### TC005: Field Accessibility
- Objective: To verify that all form fields have proper labels and accessibility attributes.
- Steps:
    1. Inspect the form elements.
- Expected Outcome: Each input has a corresponding label with a matching 'for' attribute, and appropriate 'type' and 'data-testid' attributes are present.

### TC006: Responsive Behavior
- Objective: To ensure the contact form displays and functions correctly across mobile, tablet, and desktop viewports.
- Steps:
    1. Resize the browser to different viewport sizes.
    2. Check that the form elements are visible and usable.
    3. Submit the form on each viewport size.
- Expected Outcome: The form is correctly rendered and functional on all devices.

### TC007: Admin Message Modal Display
- Objective: To verify that clicking a message in the admin panel opens a modal with the correct details.
- Steps:
    1. Submit a message and navigate to the admin messages page.
    2. Click on the subject of the test message.
- Expected Outcome: A modal window appears, displaying the correct name, email, phone, subject, and message content.

### TC008: Admin Message Modal Closing
- Objective: To ensure the message detail modal in the admin panel can be closed correctly.
- Steps:
    1. Open the message detail modal in the admin panel.
    2. Click the 'Close' or 'X' button.
- Expected Outcome: The modal window disappears. 