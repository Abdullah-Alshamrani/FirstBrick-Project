# FirstBrick Project

A robust API system designed for real estate investment platforms, enabling secure user authentication, project creation, investments, payments, and portfolio management.

---

## 🚀 Features
- **User Management**: Register, log in, and manage user profiles securely using JWT-based authentication.
- **Project Management**: Create and view real estate investment projects.
- **Investment**: Enable users to invest in projects with real-time balance checks.
- **Payment Gateway**: Seamless integration for Apple Pay top-ups and transaction tracking.
- **Portfolio Management**: View user-specific investment details and portfolios.
- **RabbitMQ Integration**: Efficient asynchronous message handling.

---

📜 Key API Endpoints

## Account


-- **Account Service**
POST /v1/user : Register a new user.
POST /v1/login : Authenticate and return JWT token.
GET /v1/user/{user_id} : Fetch user profile information.
PUT /v1/user/{user_id} : Update user profile.
**Investment Service**
POST /v1/project : Create a new real estate project.
GET /v1/projects : View all projects.
POST /v1/invest : Contribute to a project.
**Portfolio Service**
GET /v1/portfolio : View user's investment portfolio.
GET /v1/portfolio/{project_id} : View specific project details in the
portfolio.
**Payment Service**
POST /v1/ApplepayTopup : Mock endpoint, add funds to the user account
directly..
GET /v1/balance : returns the user balance.
GET /v1/transactions : returns a paginated list of user transactions.
