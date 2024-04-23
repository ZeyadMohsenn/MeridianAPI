## Store Project Requirements

1. User Authentication and Authorization:
    - Users must be able to register, login, and logout.
    - Different roles should be implemented such as admin, trainer, and customer, each with specific permissions.

2. Category Management:
    - Admins can create, update, and delete categories.
    - Categories can have subcategories.
    - Categories and subcategories have a name, description and photo.

3. Product Management:
    - Admins can add, edit, and remove products.
    - Products should belong to a category and subcategory.
    - Each product should have a name, description, price, photo, and stock quantity.

4. Order Management:
    - Customers should be able to add products to their shopping cart.
    - Customers can place orders with multiple products.
    - Orders should calculate total amounts, including taxes and any applicable discounts.
    - Orders should have a status such as `inProgress`, `shipped`, or `cancelled`.
    - Admins should be able to view and manage orders, including marking them as shipped or cancelled or inProgress.

5. Discount and Tax Calculation:
    - Discounts can be applied either as percentage discounts or fixed amount discounts.
    - Taxes will be fixed at 5%.

6. Invoice Generation:
    - Automatically generate invoices for each order placed.
    - Invoices should contain details of the products ordered, total amounts, taxes, discounts, and payment information.
    - Invoices should Saved to the database.