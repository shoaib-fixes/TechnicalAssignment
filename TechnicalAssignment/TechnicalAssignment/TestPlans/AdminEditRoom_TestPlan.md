# Test Plan: Admin Room Editing

## 1. Overview

This plan details the tests for editing existing rooms in the admin panel.

## 2. Test Cases

### TC001: Navigate to Room Details Page
- Objective: Verify that clicking a room number navigates to its details page.
- Steps:
    1. Create a test room.
    2. Click the room number link in the list.
- Expected Outcome: The room details page for the correct room is displayed.

### TC002: Verify Room Details Display
- Objective: Ensure all room details are displayed correctly on the details page.
- Steps:
    1. Create a room with specific details (type, accessibility, price, features).
    2. Navigate to its details page.
- Expected Outcome: All details on the page match the details of the created room.

### TC003: Edit Room Fields
- Objective: Verify that modifying room fields and saving works correctly.
- Steps:
    1. Navigate to a room's details page.
    2. Click 'Edit', change the type, accessibility, price, and features.
    3. Click 'Update'.
- Expected Outcome: The page updates to show the new details.

### TC004: Cancel Edit Operation
- Objective: Ensure that canceling an edit reverts all changes.
- Steps:
    1. On the room details page, click 'Edit'.
    2. Change several fields but click 'Cancel' instead of 'Update'.
- Expected Outcome: The room's details remain unchanged.

### TC005: Edit Room Number – Valid Update
- Objective: Verify that a room's number can be successfully updated to a new, valid number.
- Steps:
    1. Navigate to a room's details page and click 'Edit'.
    2. Change the room number to a new, unused number.
    3. Click 'Update'.
- Expected Outcome: The room number is updated.

### TC006: Edit Room Number – Duplicate Number Rejection
- Objective: Ensure the system prevents updating a room number to an existing number.
- Steps:
    1. Create two rooms.
    2. Attempt to change the number of the second room to the number of the first room.
- Expected Outcome: An error message is displayed.

### TC007: Edit Room Number – Invalid Number Rejection
- Objective: Ensure the system rejects invalid room numbers.
- Steps:
    1. Attempt to update a room number to a non-numeric value (e.g., 'invalid-room').
- Expected Outcome: An error message is displayed.

### TC008: Edit Room Number – Large Number Over 1000
- Objective: Ensure room numbers greater than 1000 are rejected.
- Steps:
    1. Attempt to update a room number to a value over 1000.
- Expected Outcome: An error message is displayed, specifying the valid range.

### TC009: Invalid Image URL on Edit
- Objective: Verify the system handles an invalid image URL gracefully.
- Steps:
    1. Edit a room and provide a nonsensical string for the image URL.
    2. Save the changes.
- Expected Outcome: The system does not use the invalid URL and may fall back to a default image. 