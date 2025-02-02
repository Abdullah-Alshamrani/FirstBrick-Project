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

**Account
Method	Endpoint	Description
POST	/api/account/login	User login
POST	/api/account/register	User registration
GET	/api/account	Get all users (Admin only)
GET	/api/account/{id}	Get user by ID
PUT	/api/account/{id}	Update user profile
Investment
Method	Endpoint	Description
POST	/api/investment/project	Create a new project (Authorized)
GET	/api/investment/projects	Get all investment projects
POST	/api/investment/invest	Invest in a project (Authorized)
Payment
Method	Endpoint	Description
POST	/api/payment/applepaytopup	Top up using Apple Pay
GET	/api/payment/balance	Get user balance
GET	/api/payment/transactions	Get user transactions
Portfolio
Method	Endpoint	Description
GET	/v1/portfolio	Get user portfolio
GET	/v1/portfolio/{projectId}	Get portfolio details by project
