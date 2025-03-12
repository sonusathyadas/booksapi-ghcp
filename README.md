# Book API

This is a simple Book API built using .NET Core 8, which allows for CRUD operations on a Book model. The API uses SQLite as the database and Entity Framework Core for data access.

## Project Structure

- **BookApi.sln**: Solution file that organizes the project and its dependencies.
- **Controllers/BooksController.cs**: Contains the `BooksController` class for handling HTTP requests related to the Book model.
- **Data/BookContext.cs**: Defines the `BookContext` class for accessing the Book entities in the database.
- **Models/Book.cs**: Represents the Book model with properties such as Id, Title, Author, Language, and Category, including data validation annotations.
- **Migrations/**: Contains migration files for managing database schema changes.
- **Program.cs**: Entry point of the application that sets up the web host and runs the application.
- **appsettings.json**: Configuration settings for the application, including the SQLite database connection string.
- **Startup.cs**: Configures services and the application's request pipeline.
- **BookApi.Tests/**: Contains unit tests for the API, ensuring the controller behaves as expected.

## Setup Instructions

1. Clone the repository or download the project files.
2. Open the project in your preferred IDE.
3. Restore the NuGet packages by running:
   ```
   dotnet restore
   ```
4. Update the database by applying migrations:
   ```
   dotnet ef database update
   ```
5. Run the application:
   ```
   dotnet run
   ```

## Usage

Once the application is running, you can access the API endpoints to perform CRUD operations on the Book model. Use tools like Postman or curl to interact with the API.

## Endpoints

- `GET /api/books`: Retrieve all books.
- `GET /api/books/{id}`: Retrieve a book by its ID.
- `POST /api/books`: Create a new book.
- `PUT /api/books/{id}`: Update an existing book.
- `DELETE /api/books/{id}`: Delete a book by its ID.

## License

This project is licensed under the MIT License.