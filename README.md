# BlogPost API (.NET)

A RESTful API built with ASP.NET Core for managing blog posts, user accounts, comments, tags, and notifications. Designed for scalability, clean architecture, and easy integration with frontend applications.

---

## Features

- 📝 CRUD operations for blog posts
- 👥 User registration & login with JWT-based authentication
- 💬 Commenting system linked to posts
- 🏷️ Tag management 
- 🔔 Notifications for user interactions
- 📦 MongoDB for document-based storage
- 📁 Modular folder structure
- 📃 Consistent API response model

---

## Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 6/7)
- **Database:** MongoDB (via MongoDB.Driver)
- **Authentication:** JWT (JSON Web Tokens)
- **Data Transfer:** DTO pattern
- **Serialization:** System.Text.Json / Newtonsoft.Json
- **IDE:** Visual Studio / Visual Studio Code

---

## Project Structure

```
blogpost-api/
│
├── Controllers/        # API Endpoints
├── Entity/             # Data models (PostEntity, UserEntity, etc.)
├── Services/           # Business logic
├── DTOs/               # Data Transfer Objects
├── Utils/              # Helper classes (Email Service, OTP service)
├── Security/           # Security classes (JWT, Password encrypt)
├── appsettings.json    # App configuration (MongoDB connection, etc.)
└── Program.cs          # Entry point
```

---

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download)
- [MongoDB](https://www.mongodb.com/try/download/community) installed locally or use [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)

### Steps

1. **Clone the repository**

   ```bash
   git clone https://github.com/GavinHemsada/blogpost-api-dotnet.git
   cd blogpost-api-dotnet
   ```

2. **Configure MongoDB**

   In `appsettings.json`, update the `ConnectionString` and `DatabaseName` fields:

   ```json
   "MongoDB": {
     "ConnectionString": "mongodb://localhost:27017",
     "DatabaseName": "BlogPostDB"
   }
   ```

3. **Run the project**

   ```bash
   dotnet run
   ```

4. **Test endpoints using Postman or Swagger**

   By default, the API runs on `https://localhost:5001` or `http://localhost:5000`.

---

##  API Endpoints Overview

###  Authentication

| Method | Endpoint              | Description           |
|--------|------------------------|------------------------|
| POST   | `/api/auth/register`   | Register a new user    |
| POST   | `/api/auth/login`      | Login & get JWT token  |

###  Blog Posts

| Method | Endpoint          | Description             |
|--------|-------------------|-------------------------|
| GET    | `/api/posts`      | Get all blog posts      |
| GET    | `/api/posts/{id}` | Get a single post       |
| POST   | `/api/posts`      | Create a new post       |
| PUT    | `/api/posts/{id}` | Update a post           |
| DELETE | `/api/posts/{id}` | Delete a post           |

###  Comments

| Method | Endpoint                  | Description           |
|--------|---------------------------|-----------------------|
| GET    | `/api/comments/post/{id}` | Get comments by post  |
| POST   | `/api/comments`           | Add a comment         |

###  Tags

| Method | Endpoint     | Description              |
|--------|--------------|--------------------------|
| GET    | `/api/tags`  | Get all tags             |
| POST   | `/api/tags`  | Create a new tag         |

###  Notifications

| Method | Endpoint             | Description           |
|--------|----------------------|-----------------------|
| GET    | `/api/notifications` | Get user notifications|

---

##  JWT Authentication

To access protected routes, include the token in your request headers:

```
Authorization: Bearer {your-token}
```

---

##  Postman Collection

You can find a ready-to-use Postman collection in the `/postman` folder (if added). Import it to test all endpoints easily.

---

##  Contributing

Contributions are welcome! Please fork the repo and submit a pull request.

---

##  License

This project is licensed under the [MIT License](LICENSE).

---

##  Author

**Gavin Hemsada** – [@GavinHemsada](https://github.com/GavinHemsada)
