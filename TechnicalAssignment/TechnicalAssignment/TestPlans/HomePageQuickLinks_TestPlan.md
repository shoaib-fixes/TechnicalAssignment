# Test Plan: Home Page Quick Links

## 1. Overview

This test plan validates the "Quick Links" section in the footer of the homepage.

## 2. Test Cases

### TC001: Section Visibility
- Objective: To verify that the Quick Links section is present and visible.
- Steps:
    1. Scroll to the footer of the page.
- Expected Outcome: The "Quick Links" section and its header are visible.

### TC002: Header Text
- Objective: To ensure the header text is correct.
- Steps:
    1. Get the text of the Quick Links section header.
- Expected Outcome: The text is exactly "Quick Links".

### TC003: Link Properties and Behavior
- Objective: To verify that each link has the correct text, href, and is clickable.
- Steps:
    1. For each link (Home, Rooms, Booking, Contact), inspect its properties.
- Expected Outcome: Each link has the correct text and a valid href attribute.

### TC004: List Structure
- Objective: To ensure the Quick Links are structured correctly in an unordered list.
- Steps:
    1. Inspect the HTML structure of the Quick Links section.
- Expected Outcome: There is a `<ul>` element containing exactly 4 `<li>` elements.

### TC005: All Links Presence and Order
- Objective: To verify that all expected links are present and in the correct order.
- Steps:
    1. Get the text of all links in the section.
- Expected Outcome: The links are "Home", "Rooms", "Booking", and "Contact", in that order.

### TC006: Link Text Content
- Objective: To verify the exact text content of each individual link.
- Steps:
    1. Get the text of each link one by one.
- Expected Outcome: The text matches the link's purpose (e.g., "Home" for the home link).

### TC007: Responsive Display on Mobile
- Objective: To verify the Quick Links section displays correctly on mobile viewports.
- Steps:
    1. Resize the browser to a mobile viewport.
    2. Scroll to the Quick Links section.
- Expected Outcome: All links are visible, readable, and functional.

### TC008: Responsive Display on Tablet
- Objective: To verify the Quick Links section displays correctly on tablet viewports.
- Steps:
    1. Resize the browser to a tablet viewport.
    2. Scroll to the Quick Links section.
- Expected Outcome: All links are visible, readable, and functional.

### TC009: Responsive Display on Desktop
- Objective: To verify the Quick Links section displays correctly on desktop viewports.
- Steps:
    1. Resize the browser to a desktop viewport.
    2. Scroll to the Quick Links section.
- Expected Outcome: All links are visible, readable, and functional. 