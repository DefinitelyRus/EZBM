# Tasks

*Note: All frontend changes are for Razor Pages only.*

## General Layout & Components

- [x] Adjust main content width for screens 1600px wide or more when the sidebar is open
- [x] Keep sidebar icons visible, clickable, and aligned properly when the sidebar is closed
- [x] Remove the menu button from the top left on smaller screens
- [x] Add a collapse button as the first option in the sidebar tab list
- [x] Add an extend button in the same position when the sidebar is closed
- [x] Add options to hide the mode toggle button and use standard form as default in the wizard component
- [x] Fix footer placement in portrait mode so it stays at the bottom of the page
- [ ] Add a simple description hint below the fields in all add and edit wizards

## Inventory

- [x] Move target stock and low stock limit fields into the item wizard
- [x] Ask for unit of measurement before stock limit questions in the item wizard
- [x] Skip stock limit questions in the item wizard if the unit of measurement is unlimited
- [x] Remove the hint about using negative one for unlimited stock from both forms and wizards
- [x] Apply the stock field changes to both the item wizard and the standard item form
- [x] Use friendly labels and questions for inventory fields
- [x] Remove colons from header labels that are questions or statements
- [x] Fix the form wizard component layout when switching modes so buttons stay at the bottom and viewed fields are above them

## Cashier (POS Terminal)

- [x] Investigate and fix why added items do not show up in the shopping cart list, and update `results.md`
- [x] Make the shopping cart a collapsible right sidebar on smaller screens instead of placing it at the top

## Staff & Roles

- [x] Convert the role creation form to use the wizard component
- [x] Add edit and delete buttons to role list rows that only show on hover

## Backend, Performance & Refactoring

- [x] Cache user preferences in memory to avoid repeated disk reads during date formatting
- [x] Only query database tables for the active tab in the logs page
- [x] Make database seeding run only when the database is empty to prevent data loss on startup
- [x] Refactor the wizard component to hide/show steps using CSS display property instead of moving DOM elements
- [ ] Create a base page model class to handle login checks for authorized pages
- [x] Enable cascade deletion for sales and transactions in database context
- [x] Consolidate host API routes in `Program.cs` using generic handlers
- [ ] Combine staff adjustments and payroll records into a single ledger in backend
- [x] Merge payroll and staff payment subtabs into one tab with a dynamic wizard
- [ ] Reintroduce commission tracking feature under new design
