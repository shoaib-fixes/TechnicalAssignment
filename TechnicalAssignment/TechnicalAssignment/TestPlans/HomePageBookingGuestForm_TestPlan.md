# Test Plan: Home Page Booking - Guest Form

## 1. Overview

This test plan is for the guest information form on the reservation page, which is the final step of the booking process.

## 2. Test Cases

### TC001: Empty Form Submission
- Objective: Verify that submitting a completely empty guest form triggers validation errors.
- Steps:
    1. Navigate to the reservation page and open the guest form.
    2. Click 'Submit' without entering any data.
- Expected Outcome: Validation errors are displayed for all required fields.

### TC002: Individual Field Validation
- Objective: Ensure each field in the guest form validates invalid data correctly.
- Steps:
    1. For each field (First Name, Last Name, Email, Phone), enter invalid data (e.g., numbers in name, invalid email format).
    2. Submit the form.
- Expected Outcome: A specific validation error is shown for the invalid field, and the booking is not completed.

### TC003: Boundary Case Validation
- Objective: Test the form's validation with boundary data like empty strings, whitespace, and overly long inputs.
- Steps:
    1. Enter boundary case data into each field one by one.
    2. Submit the form.
- Expected Outcome: Validation errors are displayed, and the booking is prevented.

### TC004: End-to-End Booking with Valid Data
- Objective: Verify the entire booking workflow completes successfully with valid guest information.
- Steps:
    1. Select dates and a room, then proceed to the guest form.
    2. Fill out all fields with valid information.
    3. Submit the form.
- Expected Outcome: A booking confirmation is displayed.

### TC005: Cancel Button Functionality
- Objective: Ensure the 'Cancel' button closes the guest form and returns to the previous state.
- Steps:
    1. Open the guest form and enter some data.
    2. Click the 'Cancel' button.
- Expected Outcome: The guest form is hidden, and the user is returned to the room and date selection view.

### TC006: Double Booking Prevention - Availability
- Objective: Verify that once a room is booked for specific dates, it is no longer available for booking on those same dates.
- Steps:
    1. Book a specific room for a set of dates.
    2. After confirmation, return to the homepage and search for availability using the exact same dates.
- Expected Outcome: The previously booked room does not appear in the search results.

### TC007: Forced Double Booking via URL
- Objective: Ensure the system prevents a double booking even if a user manipulates the URL to access an already-booked room.
- Steps:
    1. Book a room and note the reservation URL.
    2. After confirmation, navigate to the same URL again.
    3. Attempt to complete a second booking for the same room and dates.
- Expected Outcome: The system displays a validation error and prevents the second booking.

### TC008: Return Home Button
- Objective: Verify the 'Return Home' button on the booking confirmation page navigates back to the homepage.
- Steps:
    1. Complete a booking to get to the confirmation screen.
    2. Click the 'Return Home' button.
- Expected Outcome: The user is redirected to the site's homepage. 