# Test Plan: Home Page Booking

## 1. Overview

This test plan validates the booking functionality directly on the homepage, focusing on the date picker, availability check, and responsive design.

## 2. Test Cases

### TC001: Booking Section Elements
- Objective: To verify that the booking section is visible and contains all required elements.
- Steps:
    1. Scroll to the booking section.
- Expected Outcome: The form with date pickers, an availability button, and a title is present.

### TC002: Default Booking Dates
- Objective: To ensure the check-in and check-out date fields have valid default values upon page load.
- Steps:
    1. Observe the default dates in the booking form.
- Expected Outcome: The dates are valid, and the check-out date is on or after the check-in date.

### TC003: Check Availability with Default Dates
- Objective: To verify that clicking 'Check Availability' with default dates populates the room list.
- Steps:
    1. Click the 'Check Availability' button without changing the dates.
- Expected Outcome: The room list section updates to show available rooms.

### TC004: Custom Future Date Selection
- Objective: To ensure users can select a custom future date range and see correct availability.
- Steps:
    1. Select a valid future check-in and check-out date.
    2. Click 'Check Availability'.
- Expected Outcome: The room list updates, showing rooms available for the selected range.

### TC005: Past Date Validation
- Objective: To ensure the system prevents or handles bookings for past dates.
- Steps:
    1. Attempt to select a check-in date in the past.
- Expected Outcome: The system either prevents the selection or shows no availability.

### TC006: Invalid Date Order Validation
- Objective: To verify that the system prevents booking if the check-out date is before the check-in date.
- Steps:
    1. Select a check-out date that is earlier than the check-in date.
- Expected Outcome: The system prevents the booking or shows no availability.

### TC007: Same-Day Booking Validation
- Objective: To ensure the system prevents one-night bookings where check-in and check-out are on the same day.
- Steps:
    1. Select the same date for both check-in and check-out.
- Expected Outcome: The system prevents the booking or shows no availability.

### TC008: "Book Now" Navigation
- Objective: To verify that clicking a 'Book now' button on a room card navigates to the reservation page.
- Steps:
    1. After checking availability, click 'Book now' on any available room.
- Expected Outcome: The user is redirected to the corresponding reservation page.

### TC009: Responsive Display on Mobile
- Objective: To verify the booking form displays correctly on mobile viewports.
- Steps:
    1. Resize the browser to a mobile viewport.
    2. Scroll to the booking form.
- Expected Outcome: All form elements are visible, usable, and correctly styled.

### TC010: Responsive Display on Tablet
- Objective: To verify the booking form displays correctly on tablet viewports.
- Steps:
    1. Resize the browser to a tablet viewport.
    2. Scroll to the booking form.
- Expected Outcome: All form elements are visible, usable, and correctly styled.

### TC011: Responsive Display on Desktop
- Objective: To verify the booking form displays correctly on desktop viewports.
- Steps:
    1. Resize the browser to a desktop viewport.
    2. Scroll to the booking form.
- Expected Outcome: All form elements are visible, usable, and correctly styled. 