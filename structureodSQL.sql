-- Abdullah Alshamrani

-- 01/30/2025

-- FirstBrick Project: Database Schema : VILLA CAPITAL

-- *************************************************************




-- *************************************************************

-- Create 'users' table (Account Service)

-- This table stores all users, including regular users (investors) and project owners

-- *************************************************************

CREATE TABLE users (

    user_id SERIAL PRIMARY KEY,

    username VARCHAR(100) UNIQUE NOT NULL,

    password_hash TEXT NOT NULL,

    full_name VARCHAR(255) NOT NULL,

    email VARCHAR(255) UNIQUE NOT NULL,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);


-- *************************************************************

-- Create 'projects' table (Investment Service)

-- *************************************************************

CREATE TABLE projects (

    project_id SERIAL PRIMARY KEY,

    owner_id INT REFERENCES users(user_id),

    title VARCHAR(255) NOT NULL,

    description TEXT,

    goal_amount DECIMAL(12, 2) NOT NULL CHECK (goal_amount > 0),

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);


-- *************************************************************

-- Create 'investments' table

-- *************************************************************

CREATE TABLE investments (

    investment_id SERIAL PRIMARY KEY,

    user_id INT REFERENCES users(user_id),

    project_id INT REFERENCES projects(project_id),

    amount DECIMAL(12, 2) NOT NULL CHECK (amount > 0),

    invested_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);


-- *************************************************************

-- Create 'transactions' table (Payment Service)

-- *************************************************************

CREATE TABLE transactions (

    transaction_id SERIAL PRIMARY KEY,

    user_id INT REFERENCES users(user_id),

    type VARCHAR(50) CHECK (type IN ('top-up', 'investment')) NOT NULL,

    amount DECIMAL(12, 2) NOT NULL CHECK (amount > 0),

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);


-- *************************************************************

-- Create 'portfolio' table (Portfolio Service)

-- *************************************************************

CREATE TABLE portfolio (

    portfolio_id SERIAL PRIMARY KEY,

    user_id INT REFERENCES users(user_id),

    project_id INT REFERENCES projects(project_id),

    investment_total DECIMAL(12, 2) NOT NULL DEFAULT 0

);


-- *************************************************************

-- Create 'user_balance' table

-- *************************************************************

CREATE TABLE user_balance (

    user_id INT PRIMARY KEY REFERENCES users(user_id),

    balance DECIMAL(12, 2) NOT NULL DEFAULT 0,

    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP

);


-- *************************************************************

-- Sample Data Insertion (Optional)

-- *************************************************************


-- Insert sample users

-- Adding a test user, to test a user investing in one of the projects.


INSERT INTO users (username, password_hash, full_name, email)

VALUES

('Abdullah01', 'Villa01', 'Abdullah Alshamrani', 'Abdullah.Alshamrani.A@gmail.com'),

('Meshal01', 'Meshal2025', 'Meshal alshammari', 'meshal.villa@gmail.com'),

('TestUser01', 'TestPassword123', 'Test User', 'test.user@example.com');




-- Insert sample projects

INSERT INTO projects (owner_id, title, description, goal_amount)

VALUES

(1, 'NEOM: VILLA CAPITAL APARTMENTS', 'A project for eco-friendly apartments.', 98000.00),

(1, 'RIYADH: AL-YARMOOK APARTMENTS', 'A project for eco-friendly apartments.', 1980000.00),

(2, 'NEOM: VILLA CAPITAL APARTMENTS', 'A project for eco-friendly apartments.', 98000.00),

(2, 'JEDDAH: Downtown Office Space', 'A modern office complex.', 500000.00);



-- *************************************************************

-- Testing Relationships

-- *************************************************************


-- Find all projects owned by a user

SELECT * FROM projects WHERE owner_id = 1;

SELECT * FROM projects WHERE owner_id = 2;


-- Find all investments made by a user

SELECT * FROM investments WHERE user_id = 2;





-- *************************************************************

-- Case Senario: User 'TestUser01' would love to invest in Abdullah's project in neom.

-- In this case senario, "TESTUSER" is investing 6800SR in Abdullah's project and date of investments is recorded.

-- *************************************************************

SELECT user_id FROM users;


-- shows that "TestUsers" has an ID '4'.


SELECT project_id FROM projects WHERE title = 'NEOM: VILLA CAPITAL APARTMENTS' AND owner_id = 1;


-- insrting "TestUser" investment of 6800SR

INSERT INTO investments (user_id, project_id, amount, invested_at)

VALUES (4, 6, 6800, CURRENT_TIMESTAMP);



-- running this query will show the investments table and it would show "TestUser's" investment that was just made.

SELECT * FROM investments


-- *************************************************************

-- Query to look at the total investments that was made in a specific project. in this query the project would be "Abdullah's Neom apartments" which have a project_id '6'

-- *************************************************************

SELECT SUM(amount) AS total_investment

FROM investments

WHERE project_id = 6;




-- *************************************************************

-- Query to List All Investors of a specific Project. 

-- *************************************************************


SELECT DISTINCT user_id

FROM investments

WHERE project_id = 6;




-- *************************************************************

-- End of Schema

-- *************************************************************





-- DEVELOPER NOTE 


-- *************************************************************

-- Please know that the functional abilities would be implemented in the application layer (.NET core), 

-- and what I mean by that is for instance, in the case of "TestUser," an investment of 6800 RS 

-- was successfully recorded in the database without verifying the user's available balance.

-- This issue should be looked at and a resolve should be implementes in (.NET CORE).

-- While it is technically possible to do such a thing at the database level 

-- thorugh triggeres or stored procedures.

-- it is not a good practice to do such a thing in the database level.

-- *************************************************************


