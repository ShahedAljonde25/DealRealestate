# Deal Real Estate

## About the Project

Deal Real Estate is a web application for buying and selling land in Jordan, mainly in West Amman and Dabouq.

The system allows users to view land listings, explore property details, save favorite properties, and communicate with other users.

The project has a backend built with ASP.NET Core Web API and a frontend built with HTML, CSS, and JavaScript.

## Main Features

- User registration and login.
- Secure user authentication.
- View available land listings.
- Add, update, and delete property listings.
- View property details.
- Add properties to favorites.
- Send and receive messages.
- Upload and view property images.
- Support for panoramic property media.
- Control user access based on roles and permissions.
- Connect the frontend with the backend using RESTful APIs.

## Technologies Used

### Backend
- C#
- ASP.NET Core Web API
- .NET 8
- SQL Database
- RESTful APIs
- JSON
- DTOs
- JWT Authentication
- Role-Based Authorization
- Policy-Based Authorization
- Refresh Tokens
- Rate Limiting
- Logging and Auditing

### Frontend
- HTML
- CSS
- JavaScript

## Backend Implementation

The backend was developed using ASP.NET Core Web API and C#.

The project uses RESTful API principles to connect the frontend with the backend.

### RESTful API

The backend includes:

- GET: Get data from the server.
- POST: Send new data to the server.
- PUT: Update existing data.
- DELETE: Delete data.

Other implemented concepts include:

- JSON and XML data formats.
- HTTP status codes.
- Request and response handling.
- DTOs for data transfer.
- File upload and retrieval.
- Client-server communication.
- Three-tier architecture.

### Security

The project includes different security features:

- **JWT Authentication:** Verify users when they log in.
- **Role-Based Authorization:** Give users access based on their roles.
- **Policy-Based Authorization:** Apply specific access rules.
- **Ownership-Based Authorization:** Allow users to access or manage their own resources.
- **Refresh Tokens:** Support user sessions and token renewal.
- **Rate Limiting:** Limit the number of requests to the API.
- **Logging and Auditing:** Record important system activities.

These features help protect the API and control access to system resources.

## Project Architecture

The backend uses a layered architecture to organize the code and separate responsibilities.

Main parts include:

- **API Layer:** Receives HTTP requests and sends responses.
- **Business Layer:** Handles business rules and application logic.
- **Data Access Layer:** Handles database operations.
- **Models and DTOs:** Define application data and transfer data between layers.

This structure makes the code easier to manage, understand, and update.

## Project Structure

```text
DealRealestate/
│
├── BusinessLayer/
├── Connection/
├── DataAccess/
├── DataAccsessLayer/
├── DataLayer/
├── DealRealState.API/
├── DealRealState.DTOs/
├── DTOs/
├── Models/
│
├── Frontend/
│
├── DVLDV.sln
├── DataLayer.sln
├── .gitignore
└── README.md
```

*Note: This structure shows the main project folders. Some additional files and folders may exist.*

## How to Run the Project

### Backend

1. Open the project using Visual Studio.
2. Open `DVLDV.sln`.
3. Configure the database connection.
4. Restore the NuGet packages.
5. Build the solution.
6. Run the ASP.NET Core Web API project.

### Frontend

1. Open the `Frontend` folder.
2. Open `index.html` in your browser.

## Project Goal

The goal of this project is to build a real estate web application using modern web development technologies.

It combines backend development, frontend development, database operations, RESTful APIs, and API security in one practical project.

## Project Status

The project is under development. New features and improvements may be added.
