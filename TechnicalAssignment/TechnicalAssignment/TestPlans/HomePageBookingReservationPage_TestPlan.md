# Test Plan: Home Page Booking - Reservation Page

## 1. Overview

This document outlines the tests for the reservation page, which appears after a user selects a room to book from the homepage.

## 2. Test Cases

### TC001: Room Details Display
- Objective: To verify that all essential room details are correctly displayed.
- Steps:
    1. Navigate to the reservation page for any room.
- Expected Outcome: The room's title, image, description, and policies are visible and not empty.

### TC002: Guest Count Display per Room Type
- Objective: To ensure that each room type displays the correct maximum guest count.
- Steps:
    1. For each room type, navigate to its reservation page.
- Expected Outcome: The displayed guest count matches the expected count for that room type (e.g., 'Double' shows 2 guests).

### TC003: Calendar Button Functionality
- Objective: To verify that the calendar navigation buttons work as expected.
- Steps:
    1. Click 'Next', 'Back', and 'Today' on the calendar.
- Expected Outcome: The calendar month changes correctly and returns to the current month when 'Today' is clicked.

### TC004: Price Summary Calculation
- Objective: To ensure the price summary is calculated correctly.
- Steps:
    1. On the reservation page, observe the price summary.
- Expected Outcome: The total price correctly equals (Room Price × Nights) + Cleaning Fee + Service Fee.

### TC005: "Reserve Now" Button
- Objective: To verify that clicking 'Reserve Now' reveals the guest information form.
- Steps:
    1. Click the 'Reserve Now' button.
- Expected Outcome: The guest form becomes visible.

### TC006: Accessible Badge Display
- Objective: To ensure the 'Accessible' badge is displayed for rooms that are marked as accessible.
- Steps:
    1. Navigate to the reservation page for a room known to be accessible.
- Expected Outcome: An 'Accessible' badge is visible on the page.

### TC007: Similar Rooms Section
- Objective: To verify that the 'Similar Rooms' section is displayed.
- Steps:
    1. Scroll to the bottom of the reservation page.
- Expected Outcome: The 'Similar Rooms' section is visible and contains at least one other room.

### TC008: Room Features Display
- Objective: To ensure room features are listed correctly.
- Steps:
    1. Navigate to a reservation page.
- Expected Outcome: The list of room features is present and contains expected items like 'WiFi', 'TV', etc.

### TC009: Room Image Loading
- Objective: To verify that the main room image loads correctly.
- Steps:
    1. Navigate to a reservation page.
- Expected Outcome: The room image is visible and its source URL is valid and loads without errors.

### TC010: URL Parameter Handling
- Objective: To ensure that booking parameters like room ID, check-in, and check-out dates are correctly handled in the URL.
- Steps:
    1. Select dates and a room from the homepage to proceed to the reservation page.
- Expected Outcome: The URL contains the correct path, room ID, and date parameters. 