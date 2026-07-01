# Layout Plan

## Login Page

The login page will be a simple, generic login page with a username and password fields.

The header will be visible, with only the title "EZBM". This title can be configured in the app settings.

The center of the page will have the login form.

It will have a centered "Login" title, with a card containing the username and password fields and the login button.

### First-time setup

On first-time setup, the login page will be skipped and instead replaced with an onboarding / first-time setup page. The onboarding page will contain the following fields:

- Business Name (text field)
- Initial Admin Username (text field)
- Initial Admin Password (password field)
- Confirm Initial Admin Password (password field)
- First Name (text field)
- Last Name (text field)
- Email (validated text field, optional)
- Phone Number (validated text field, optional)
- Position (text field, optional)
- Pay Frequency (dropdown, default to none (= "invalid"))
- Pay Rate (float field, default to 0.00)
- Currency (dropdown w/ current locale's currency as default)
- "Continue to App" button

Clicking the "Continue to App" button automatically adds a new user with the given information and then proceeds to the dashboard, already logged in. From here, the user experience will be identical to that of a user who just logged in.

## Main Page

The main page will have a header, collapsible sidebars on both sides, and a main content area in the center, much like Facebook.

The left sidebar will contain navigation links to the different pages of the application, ordered logically.

The right sidebar will display quick action buttons, such as "Add Item", "Add Sale", "Clock In", "Clock Out", etc. in the case of the dashboard tab. The contents of this sidebar will change depending on the active tab.

The main content area will display the content of the current page. This content can be configured in the user preferences page.

There will be a total of 8 tabs that the main content area can display:

- Dashboard
- Inventory
- Point of Sale
- Analytics
- Payroll
- Settings
- Staff

### Dashboard

The dashboard is a scrollable feed of cards, each displaying a different metric or list. By default, the feed will display:

- Current date and time
- Total sales
- Total profit
- Total items sold
- Low stock items list

Each of these cards will be clickable, and will take the user to the corresponding page.

The "Low stock items list" card will display the top 10 low-stock items (if the item's stock is less than or equal to the low stock threshold), sorted by stock quantity. If there are no low-stock items, the card will display a message saying "No low stock items."

At the bottom, there will be a button to "Add a new card". This will open a dialog with a list of all available cards to display.

Cards can be re-ordered by holding for a duration (configurable, default 200ms) and then dragging the card to the desired position. Cards can also be removed by clicking the "X" button in the top-right corner of the card, which prompts confirmation from the user if clicked. Completing either action will save the card layout to the user's preferences automatically.

### Inventory

The inventory tab will display a sortable, filterable grid of all inventory items. Similar to how it's displayed in Amazon.

When hovering over an inventory item, a hamburger menu button will appear on the top-right corner of the item card. If clicked, a menu will drop down and display the following options:

- Add to cart
- Edit item
- Delete item
- View item details

### Point of sale

The point of sale tab will display a search bar, and a list of inventory items that can be added to the cart.

#### Inventory list

If the search bar is empty, the inventory list will display all items.

The inventory list will be scrollable and will display the item's name, image, and price.

When hovering over an inventory item, an "Add to Cart" button will appear on the top-right corner of the item card. If clicked, a pop-up submenu will appear asking for the quantity or target price. The submenu will close once the user clicks the "Add to Cart" button within the submenu.

Clicking on the label of the item will instead open the item profile, opening a modal that displays all info about that item.

#### Cart

The cart will be displayed on the right sidebar with a scrollable list of item cards. Each card will display the item's name, image, quantity, and subtotal price.

Each item card will have a quantity field and an "X" button (remove item), both initially invisible unless hovered upon.

When not hovered over, the quantity field will display the units of measure (i.e., "3x" or "1.5 kg"). Otherwise, it will display a simple number field (i.e., no unit) and recalculate the subtotal on Enter or when no longer hovered.

When an item is added, removed, or its quantity is changed, the totals, subtotals, item prices, etc. will update automatically.

At the top of the cart, there is a "total price" display and "confirm sale" button.

When the user clicks "Confirm Sale", a "checkout" modal will pop up.

#### Checkout modal

Here, the user will be asked:

- The customer's payment method (dropdown, cash by default)
- How much they're paying (auto-filled to exact if non-cash, exact if empty)
- Additional remarks (optional)
- Whether to show a receipt (checkbox)

If the total is PHP 500.00 or higher (currency and amount configurable in admin settings), the checkbox will be pre-checked and a notice will appear above the "Confirm Sale" button. "Note: Written receipts MUST be issued for purchases totaling PHP 500.00 or higher."

#### Receipt Modal

The receipt will be a modal that displays the following information:

- Item name
- Item quantity
- Item subtotal
- Total price
- Date and time of the transaction
- Payment method
- Amount paid
- Change to give

Display the change to give (if any) on a separate line.

Clicking the "Confirm Sale" button will clear the cart, save the transaction to the database, and display the receipt containing info needed for a physical written receipt (items, quanities, subtotals, total, date, time, etc.)

If the total is PHP 500.00 or higher, the "Confirm Sale" button will be locked for 10 seconds (configurable in admin settings) and a countdown timer will appear in the "Confirm Sale" text, counting down to when the button will be unlocked (i.e., "Confirm Sale (10s)").

### Analytics

The analytics tab will display a dashboard with charts and graphs of sales, profit, and other metrics.

*To plan later...*

### Payroll

The payroll tab will display a scrollable list of staff members, each with their total hours worked for the current pay period, total pay for the current pay period, and an "Edit" button.

The list will be sortable and filterable by:

- (All staff properties)
- Total hours worked (current pay period)
- Total pay (current pay period)

The bottom of the page will have a footer displaying the total pay for all staff members for the current pay period. At the right edge of the footer, there will be a "Generate Payroll" button.

When clicked, it will calculate the total pay for all staff members for the current pay period and display it in a modal. If the total pay is greater than PHP 500.00 (configurable in admin settings), the user will be required to confirm the payroll generation by entering their password. Once confirmed, the payroll will be saved to the database and displayed in a modal.

*TODO: Incentives, Commissions*

#### Sidebar actions

##### Advance pay & bonuses

The advance pay and bonuses action will display a modal that allows the user to give advance pay or bonuses to one or more staff members.

The modal will display a list of staff members, identical to the one in the staff tab. When hovered over, a checkbox will appear on the top-left corner of the card, marking the staff as selected.

The right sidebar contains the following options:

**Pay Type:**
A dropdown menu with the options:

- Bonus (default)
- Advance Pay

This is purely for bookkeeping purposes and does not have any effects.

**Pay for select days (Optional):**
A custom date picker that allows the user to select specific days to pay for.

This is useful for:

- Holidays
- Birthday rewards
- When the staff asks for 3 days' salary (e.g. if they usually get paid on the 30th, but need to go on the 25th).

**Amount:**
A number field (default: "0.00").

**Deduct from current payroll:**
This is useful for staff-requested salary deductions (e.g. "Can I have 3 days' salary in advance?").

If checked, the amount will be deducted from their earliest workday on the current pay period, and then the next earliest, and so on, until the full amount is deducted. As such, those work days will be recorded as already paid.

**Remarks (Optional):**
A short text field for any additional information (e.g. "advance pay for birthday").

**Confirm Pay:**
A button that will add the advance pay/bonus to the selected staff member(s).

If the total amount to be given is greater than PHP 500.00 (configurable in admin settings), the user will be required to confirm the advance pay/bonus by entering their password. Once confirmed, the advance pay/bonus will be saved to the database and the modal will close.

This action is irreversible.

### Settings

*To plan later...*

### Staff

*To plan later...*

TODO: Permissions list--view/edit permissions on each tab

### Header

The header will appear across the top edge of the page, above the sidebars and main content area. It will remain visible even when scrolling, but modals will take priority over it.

The header will contain the following elements:

- Business Name (editable by an admin in business settings)
- Current User (clickable to open user menu)
  - User profile picture and name on the right.
  - Dropdown menu with the following options:
    - View profile
    - User Settings
    - Business Settings (admin only)
    - Time In/Out
    - Sign Out

The business name will be centered, while the current user section will be right-aligned.

The business name will only be visible in the login, onboarding, dashboard, and analytics tabs. It will otherwise be replaced with a search bar.

#### Search bar

All search bars will have a filter button on the right of the search bar, which will open a filter menu when clicked.

The filter menu's options/fields will vary depending on the current tab. It will usually have options/fields corresponding to the data being searched/displayed in that tab. Pressing Enter when the filter menu is open will apply the filters.

The search will be updated in real-time as the user types in the search bar, with a short delay to prevent excessive queries.

The delay is configurable in the admin settings (default 500ms, min 0ms, max 2000ms), and it will not trigger a search query unless the query is different from before.

## Other changes to make

- Add a "low stock threshold" property to inventory items.
