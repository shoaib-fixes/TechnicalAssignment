# Home Page Navigation Test Plan

## Overview
This plan tests the main navigation bar on the home page. It ensures all links work, including the brand, internal section links, and the admin page link. It also covers the responsive "hamburger" menu on smaller screens.

## Test Strategy
We use a data class to provide standard viewport sizes for responsive tests. All interactions go through a specific navigation component to keep tests clean. Helpers are used for resizing the browser and waiting for dynamic events like URL changes.

## Test Cases

### TC001: Brand link click
Objective: Ensure the brand link in the navbar works correctly.
Steps: Click the brand link/logo.
Expected: The page reloads but stays on the home page.

### TC002 - TC006: Internal section links
Objective: Test that internal links like "Rooms" and "Contact" scroll to the right section.
Steps: For each internal link, click it and verify the correct page section scrolls into view.
Expected: Clicking each link scrolls the page to the correct section.

### TC007: Admin link navigation
Objective: Confirm the "Admin" link navigates to the admin login page.
Steps: Click the "Admin" link.
Expected: The browser goes to the admin page URL.

### TC008: Navbar toggle on mobile
Objective: Test the "hamburger" menu on mobile devices.
Steps: Resize to a mobile viewport, check that links are hidden. Click the toggle to show them. Click it again to hide them.
Expected: The toggle correctly shows and hides the navigation links.

### TC009: All links visible on desktop
Objective: Confirm all navigation links are visible on a desktop screen.
Steps: On a desktop viewport, check for the visibility of all seven nav links.
Expected: All links are visible.

### TC010: Correct link text
Objective: Ensure all navigation links display the correct text.
Steps: Get the text from each of the seven navigation links.
Expected: The text of each link is correct (e.g., "Rooms").

### TC011: Navbar on tablets
Objective: Test the navbar behavior on tablet screen sizes.
Steps: Resize to a tablet viewport, click the toggle to expand the menu, then click a link.
Expected: The menu expands and navigation works correctly.

### TC012: Navbar on desktops
Objective: Confirm that on wide screens, all links are directly visible without a toggle.
Steps: Resize to a desktop viewport and check for the toggle and links.
Expected: The toggle button is not visible, and all links are directly visible in the navbar. 