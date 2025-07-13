# Test Plan: Admin Room Management

## 1. Overview

This test plan focuses on the management of rooms from the admin panel, including deletion, visibility on the public site, and access control.

## 2. Test Cases

### TC001: Bulk-Delete Sanity Check
- Objective: To verify that deleting rooms from a list of multiple rooms works as expected and does not affect other rooms.
- Steps:
    1. Create three rooms.
    2. Delete the middle room.
    3. Verify the first and third rooms are still present and the second is gone.
    4. Delete the remaining rooms.
- Expected Outcome: The list updates correctly after each deletion, and only the targeted room is removed.

### TC002: Delete Room from Listing
- Objective: To ensure a single room can be successfully deleted from the room listing page.
- Steps:
    1. Create a new room.
    2. Verify it appears in the list.
    3. Click the 'Delete' button for that room.
- Expected Outcome: The room is removed from the list.

### TC003: Verify New Rooms on Public HomePage
- Objective: To ensure that a newly created room is visible on the public-facing homepage.
- Steps:
    1. Create a room with a unique price and features.
    2. Navigate to the public homepage.
    3. Scroll to the rooms section.
- Expected Outcome: A room card matching the details of the newly created room is displayed.

### TC004: Unauthorized Access Redirect to LoginPage
- Objective: To verify that accessing the admin rooms page without being logged in redirects to the login page.
- Steps:
    1. Clear all cookies to log out.
    2. Attempt to navigate directly to the `/admin/rooms` URL.
- Expected Outcome: The user is redirected to the admin login page. 