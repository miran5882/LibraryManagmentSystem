# LibraryManagmentSystem
Overview

The Library Management System is a robust web API application built using .NET Framework 4.7.2. It provides a secure and scalable solution for managing library resources, including books, roles, and users, with role-based access control. This project leverages Entity Framework 6.0.0.0 for database interactions and JWT-based authentication to ensure secure access.

Features





User Authentication: Users can authenticate via the api/auth endpoint using their email and password, receiving a JWT token for subsequent requests.



Role-Based Authorization: Access is restricted based on user roles (Admin, InCharge, Staff, Member), defined in the Roles table.





Admin: Full access to manage books, roles, and users.



InCharge: Manage books and view roles.



Staff: Manage books only.



Member: Read-only access to books.



CRUD Operations:





Books: Create, read, update, and delete books with role-specific permissions.



Roles: Manage roles (restricted to Admin).



Users: Manage users (restricted to Admin).



CORS Support: Enabled for http://localhost:44305 to facilitate local testing.

Project Structure





Controllers:





AuthController: Handles user authentication and token generation.



BookController: Manages book-related operations.



RoleController: Manages role-related operations.



UserController: Manages user-related operations.



Models: Defined using Entity Framework Code-First approach.



Configuration: Stored in Web.config with JWT secret key and database connection.

Installation





Clone the Repository:





git clone https://github.com/yourusername/library-management-system (replace with your actual repo URL).



Open in Visual Studio:





Use Visual Studio 2019 or later with .NET Framework 4.7.2 support.



Restore Packages:





Right-click the solution > Restore NuGet Packages.



Configure Database:





Update the connection string in Web.config to point to your SQL Server instance (e.g., DESKTOP-1QQGEHD).



Ensure the Library database exists or enable migrations to create it.



Run the Project:





Press F5 to start the application using IIS Express.

Usage

Authentication





Endpoint: POST https://localhost:44305/api/auth



Request Body:

{
  "Email": "user@example.com",
  "Password": "userpass"
}



Response: Returns a JWT token on success (HTTP 200) or an error (HTTP 401/400).

Secured Endpoints





Books: https://localhost:44305/api/Book





GET: Retrieve all books (all roles).



POST: Add a book (Admin, InCharge).



PUT: Update a book (Admin, InCharge).



DELETE: Delete a book (Admin).



Roles: https://localhost:44305/api/Role





GET: Retrieve all roles (Admin, InCharge).



POST/PUT/DELETE: Manage roles (Admin only).



Users: https://localhost:44305/api/User





GET: Retrieve all users (Admin, InCharge).



POST/PUT/DELETE: Manage users (Admin only).

Testing





Tested manually using Postman with various role-based scenarios.



Ensure the API is running locally before testing.

Contributing

Feel free to fork the repository, submit issues, or create pull requests. Any feedback or suggestions are welcome!

License

This project is licensed under the MIT License - see the LICENSE file for details (add a license file if desired).

Contact

For questions or support, please contact:





Name: [Your Full Name]



Email: [Your Email Address]
