# Bug Report - Test Execution Results

Test summary: total: 152, failed: 33, succeeded: 108, skipped: 11, duration: 331.6s
Build failed with 33 error(s) in 332.5s

## Executive Summary

This bug report documents 33 test failures across multiple functional areas of the application. The failures span across performance, accessibility, admin functionality, booking system, and navigation components. 

The tests have been written with the intention that if a developer were to fix the issue, the test would start to pass without any further intervention by the QA team.

---

## 1. PERFORMANCE ISSUES

### BUG-001: Homepage Performance Below Threshold
Test: `HomePage_Performance_ShouldMeetThresholds`  
Severity: Medium  
Status: Failed  

Description:  
The homepage performance score of 0.66 is below the required threshold of 0.8 according to Lighthouse audit.

Expected Behavior:  
Performance score should be at least 0.8 as defined in the Performance Test Plan TC001.

Actual Behavior:  
Performance score is 0.66, failing to meet the minimum acceptable threshold.

Impact:  
Poor user experience due to slow page loading times.

---

## 2. ACCESSIBILITY VIOLATIONS

### BUG-002: Admin Login Page Color Contrast Violation
Test: `AdminLoginPage_Accessibility_WCAG2AA`  
Severity: High  
Status: Failed  

Description:  
WCAG 2 AA accessibility violation found on the admin login page related to color contrast.

Expected Behavior:  
No accessibility violations should be present.

Actual Behavior:  
Color contrast violation found on .btn-outline-danger element.

Impact:  
Users with visual impairments may have difficulty reading the button text.

### BUG-003: Admin Rooms Page Color Contrast Violations
Test: `AdminRoomsPage_Accessibility_WCAG2AA`  
Severity: High  
Status: Failed  

Description:  
Multiple WCAG 2 AA accessibility violations found on the admin rooms page.

Expected Behavior:  
No accessibility violations should be present according to WCAG 2 AA standards.

Actual Behavior:  
Color contrast violations found on multiple elements:
- .btn-outline-danger
- Multiple room price span elements

Impact:  
Users with visual impairments.

### BUG-004: Homepage Color Contrast Violations
Test: `HomePage_Accessibility_WCAG2AA`  
Severity: High  
Status: Failed  

Description:  
WCAG 2 AA accessibility violations found on the homepage footer links.

Expected Behavior:  
No accessibility violations should be present.

Actual Behavior:  
Color contrast violations found on footer links:
- Cookie policy link
- Privacy policy link  
- Admin link
- Other small footer links

Impact:  
Users with visual impairments.

---

## 3. ADMIN ROOM MANAGEMENT ISSUES

### BUG-005: Decimal Price Room Creation Failure
Test: `CreateRoom_DecimalPrice_ShouldSucceed`  
Severity: High  
Status: Failed  

Description:  
Room creation with decimal prices fails - room is not created and price is not saved correctly.

Expected Behavior:  
Room should be created successfully with decimal price (150.75) and appear in the room list.

Actual Behavior:  
- Room is not present in the list after creation attempt

Impact:  
Users cannot create rooms with decimal pricing despite this being a valid currency increment.

### BUG-006: Duplicate Room Number Validation Missing
Test: `CreateRoom_DuplicateRoomNumber_ShouldFail`  
Severity: High  
Status: Failed  

Description:  
System does not prevent creation of rooms with duplicate room numbers.

Expected Behavior:  
Error alert should be displayed when attempting to create a room with an existing room number.

Actual Behavior:  
No error alert is displayed, allowing duplicate room numbers.

Impact:  
Data integrity issues - multiple rooms can have the same number.

### BUG-007: Non-Numeric Room Number Validation Missing
Test: `CreateRoom_NonNumericRoomNumber_ShouldFail`  
Severity: Medium  
Status: Failed  

Description:  
System does not validate room number input to ensure it's numeric.

Expected Behavior:  
Error alert should be displayed when attempting to create a room with non-numeric room number.

Actual Behavior:  
No error alert is displayed for non-numeric room numbers.

Impact:  
Data quality issues - room numbers can contain invalid characters.

### BUG-008: Edit Room Cancel Operation Not Working
Test: `EditRoom_CancelOperation_ShouldLeaveDataUnchanged`  
Severity: Medium  
Status: Failed  

Description:  
Canceling a room edit operation does not properly revert changes.

Expected Behavior:  
Room details should remain unchanged when edit operation is canceled.

Actual Behavior:  
Room details are modified despite canceling:
- Room type changed from "Single" to "Suite"
- Accessibility changed from False to True
- Price changed from 100 to 500

Impact:  
Users may accidentally modify room data when intending to cancel changes.

### BUG-009: Edit Room Duplicate Number Validation Missing
Test: `EditRoom_DuplicateRoomNumber_ShouldFail`  
Severity: High  
Status: Failed  

Description:  
System does not prevent editing a room number to an existing number.

Expected Behavior:  
Error alert should be displayed when attempting to change room number to an existing number.

Actual Behavior:  
No error alert is displayed, allowing duplicate room numbers through editing.

Impact:  
Data integrity issues - room numbers can be duplicated through editing.

### BUG-010: Invalid Image URL Not Handled Properly
Test: `EditRoom_InvalidImageUrl_ShouldFallbackOrFailProperly`  
Severity: Low  
Status: Failed  

Description:  
System does not properly handle invalid image URLs during room editing.

Expected Behavior:  
System should either use a fallback image or reject the invalid URL.

Actual Behavior:  
Invalid URL "nonsenseURL" is accepted and included in the final image URL.

Impact:  
Broken images may be displayed to users.

### BUG-011: Edit Room Invalid Number Validation Missing
Test: `EditRoom_InvalidRoomNumber_ShouldFail`  
Severity: Medium  
Status: Failed  

Description:  
System does not validate room number format during editing.

Expected Behavior:  
Error alert should be displayed for invalid room numbers.

Actual Behavior:  
No error alert is displayed for invalid room numbers.

Impact:  
Data quality issues - room numbers can be set to invalid values.

### BUG-012: Large Room Number Validation Missing
Test: `EditRoom_LargeRoomNumber_ShouldFail`  
Severity: Medium  
Status: Failed  

Description:  
System does not validate room number range (should reject numbers over 1000 which is the validation rule for initial room creation).

Expected Behavior:  
Error alert should be displayed for room numbers over 1000.

Actual Behavior:  
No error alert is displayed for large room numbers.

Impact:  
Data quality issues - room numbers can exceed reasonable limits.

### BUG-013: New Rooms Not Appearing on Public Homepage
Test: `NewRooms_ShouldAppearOnPublicHomePage`  
Severity: High  
Status: Failed  

Description:  
Newly created rooms do not appear on the public homepage.

Expected Behavior:  
Room with type "Family" and price 200 should be visible on the public homepage.

Actual Behavior:  
Room is not found on the public page after creation.

Impact:  
Users cannot see and book newly created rooms.

---

## 4. BOOKING SYSTEM ISSUES

### BUG-014: Double Booking Prevention Not Working
Test: `AttemptForcedDoubleBooking_ShouldShowValidationError`  
Severity: Critical  
Status: Failed  

Description:  
System does not prevent double booking attempts.

Expected Behavior:  
Validation error should be displayed when attempting to book an already booked room.

Actual Behavior:  
No validation error or confirmation is shown - unexpected result - as seen from the screenshot we actually get an unfrienfly React error. 

Impact:  
Critical business logic failure - end users may not know if rooms have actually been booked or not.

### BUG-015: Guest Form Validation Issues (Multiple)
Tests: Multiple guest form validation tests  
Severity: Medium  
Status: Failed  

Description:  
Multiple guest form validation failures across different field types and validation scenarios.

Expected Behavior:  
Proper validation errors should be displayed for invalid input.

Actual Behavior:  
Validation errors are not properly displayed for various invalid inputs.

Impact:  
Users may submit bookings with invalid information.

### BUG-016: Single Room Guest Count Display Error
Test: `ReservationPage_RoomType_ShouldDisplayCorrectGuestCount`  
Severity: Medium  
Status: Failed  

Description:  
Single room displays incorrect maximum guest count.

Expected Behavior:  
Single room should display max guest count of 1.

Actual Behavior:  
Single room displays max guest count of 2.

Impact:  
Misleading information displayed to users about room capacity.

### BUG-017: Date Validation Not Working (Multiple)
Tests: Multiple date validation tests  
Severity: High  
Status: Failed  

Description:  
Date validation system is not working properly for various scenarios.

Expected Behavior:  
- Invalid date order should be rejected
- Past dates should be rejected
- Same-day booking should be rejected

Actual Behavior:  
- 3 rooms shown as available for invalid date order
- 3 rooms shown as available for past dates
- 3 rooms shown as available for same-day booking

Impact:  
Users can make bookings with invalid dates.

---

## 5. CONTACT FORM ISSUES

### BUG-018: Contact Form Field Accessibility Issue
Test: `ContactForm_FieldAccessibility_ShouldHaveProperLabelsAndAttributes`  
Severity: Medium  
Status: Failed  

Description:  
Contact form fields do not have proper accessibility attributes.

Expected Behavior:  
Each form field should have a corresponding label with matching 'for' attribute.

Actual Behavior:  
The 'for' attribute of a label does not match the 'id' of its corresponding form field.

Impact:  
Accessibility issues for screen reader users.

---

## 6. NAVIGATION ISSUES

### BUG-019: Amenities Link Not Scrolling Properly
Test: `AmenitiesLink_WhenClicked_ShouldScrollToAmenitiesSection`  
Severity: Medium  
Status: Failed  

Description:  
Amenities navigation link does not scroll to the amenities section.

Expected Behavior:  
Clicking the amenities link should scroll to the amenities section.

Actual Behavior:  
Amenities section is not visible after clicking the link due to missing #amenities section.

Impact:  
Navigation functionality is broken for amenities section.

### BUG-020: Navbar Toggle Not Visible on iPad Landscape
Test: `NavbarBehavior_OnTabletViewport_ShouldMaintainFunctionality`  
Severity: Low  
Status: Failed  

Description:  
Navbar toggler is not visible on iPad Landscape viewport.

Expected Behavior:  
Navbar toggler should be visible on iPad Landscape.

Actual Behavior:  
Navbar toggler is not visible.

Impact:  
Navigation menu not displaying as a toggle menu for iPad Landscape orientation - showing as a desktop menu instead. iPad landscape/portrait experience is inconsistent.

### BUG-021: Quick Links Href Attributes Incorrect
Tests: Multiple quick links tests  
Severity: Medium  
Status: Failed  

Description:  
Quick links in footer have incorrect href attributes.

Expected Behavior:  
- Rooms link should end with '#rooms'
- Booking link should end with '#booking'  
- Contact link should end with '#contact'

Actual Behavior:  
Links do not have the expected href values.

Impact:  
Footer navigation links do not work properly.

---

## 7. SOCIAL MEDIA ISSUES

### BUG-022: Social Media Links Not Opening in New Tab
Test: `SocialMediaLinks_ShouldOpenInNewTab`  
Severity: Low  
Status: Failed  

Description:  
Social media icons do not open in new tabs.

Expected Behavior:  
Clicking social media icons should open links in new tabs.

Actual Behavior:  
Social media links do not open in new tabs.

Impact:  
Users may lose their place on the main site when clicking social media links.

---
