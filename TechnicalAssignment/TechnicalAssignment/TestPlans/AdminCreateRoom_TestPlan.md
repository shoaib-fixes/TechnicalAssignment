# Test Plan: Admin Room Creation

## 1. Overview

This test plan covers the functionality of the room creation feature in the admin panel.

## 2. Test Cases

### TC001: Create Rooms for All Type and Accessibility Combinations
- Objective: To verify that rooms can be created with all available types and accessibility options.
- Steps:
    1. For each room type (Single, Twin, Double, Family, Suite) and accessibility (true, false):
    2. Enter a unique room number, price, and select features.
    3. Click 'Create'.
- Expected Outcome: The new room appears in the room list with the correct details.

### TC002: Room Number Field Validation – Non-Numeric Input
- Objective: To ensure the room number field rejects non-numeric input.
- Steps:
    1. Enter 'ABC' in the room number field.
    2. Fill other fields with valid data.
    3. Click 'Create'.
- Expected Outcome: An error message is displayed, and the room is not created.

### TC003: Duplicate Room Number Rejection
- Objective: To ensure the system prevents the creation of a room with a duplicate number.
- Steps:
    1. Create a room with a specific number.
    2. Attempt to create another room with the same number.
- Expected Outcome: An error message is displayed.

### TC004: Price Field Validation – Non-Numeric Input
- Objective: To ensure the price field rejects non-numeric input.
- Steps:
    1. Enter 'invalid' in the price field.
    2. Fill other fields with valid data.
    3. Click 'Create'.
- Expected Outcome: An error message is displayed.

### TC005: Required-Field Validation – Empty Inputs
- Objective: To verify that the form shows an error if mandatory fields are left empty.
- Steps:
    1. Click 'Create' without filling in any fields.
- Expected Outcome: An error message is displayed indicating mandatory fields are required.

### TC006: Price Field Validation – Invalid Low Prices
- Objective: To ensure prices less than 1 are rejected.
- Steps:
    1. Attempt to create a room with a price of 0.
    2. Attempt to create a room with a price of -100.
- Expected Outcome: An error message is displayed for both attempts.

### TC007: Price Field Validation – Decimal Price
- Objective: To verify that rooms can be created with a decimal price.
- Steps:
    1. Enter a decimal value (e.g., 150.75) in the price field.
    2. Fill other fields and click 'Create'.
- Expected Outcome: The room is created successfully, and the price is displayed correctly.

### TC008: No-Feature Selection
- Objective: To verify that a room can be created without selecting any features.
- Steps:
    1. Fill all mandatory fields but do not select any features.
    2. Click 'Create'.
- Expected Outcome: The room is created successfully.

### TC009: Default Dropdown Values on Load
- Objective: To ensure the 'Type' and 'Accessible' dropdowns have correct default values.
- Steps:
    1. Load the admin rooms page.
    2. Observe the default selected values for 'Type' and 'Accessible'.
- Expected Outcome: 'Type' should default to 'Single', and 'Accessible' should default to 'false'. 