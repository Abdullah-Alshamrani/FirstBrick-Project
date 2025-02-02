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
Account Service
POST /v1/user: Register a new user.
POST /v1/login: Authenticate user and return a JWT token.
GET /v1/user/{user_id}: Fetch user profile information.
PUT /v1/user/{user_id}: Update user profile details.
Investment Service
POST /v1/project: Create a new real estate project.
GET /v1/projects: Retrieve a list of all projects.
POST /v1/invest: Contribute to a specific real estate project.
Portfolio Service
GET /v1/portfolio: View the user's complete investment portfolio.
GET /v1/portfolio/{project_id}: View details of a specific project within the portfolio.
Payment Service
POST /v1/ApplepayTopup: Mock endpoint to add funds directly to the user's account.
GET /v1/balance: Retrieve the user's current account balance.
GET /v1/transactions: Retrieve a paginated list of the user's transaction history.
