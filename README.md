# TaxPayAI Assignment

This project consists of a **backend API** and a **frontend React application** to manage accounts and transactions. The backend API is built using .NET, while the frontend is developed using React. The application allows users to transfer money between customer and tax accounts.

## Project Structure

- **Backend API (WebApi)**: The API is built using .NET and handles all the backend logic such as account and transaction management.
- **Frontend App (React)**: The React application provides a user interface to interact with the backend API, allowing users to view accounts, transfer money, and manage transactions.

## Getting Started

### Prerequisites

- Docker
- Docker Compose

Make sure you have Docker and Docker Compose installed on your system.

### Backend API

The backend API is located in the `TaxPayApi` directory and is built using .NET. It exposes several endpoints to manage accounts and transactions.

### Frontend App

The frontend React application is located in the `app` directory. It allows users to interact with the backend API.

## How to Spin Up the App

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd <repository-directory>
```

### Step 2: Build and Run the Application Using Docker

You can use Docker Compose to build and run both the frontend and backend services.

1. **Build the services**:
   ```bash
   docker-compose up --build
   ```
2. **Access the frontend application**:
   Once the services are up, you can access the frontend React app by

```bash
http://localhost:3000
```

3. ** Access the backend API**: The API is exposed on:

```bash
http://localhost:5001
```

### Step 3: Access API Endpoints

You can use tools like Postman or curl to test the API endpoints. Below are the key endpoints provided by the backend API:

- **POST /connect/token**: Obtain an authentication token to access secured endpoints.
- **GET /api/account**: Retrieve a list of all accounts (Customer and Tax accounts).

- **POST /api/transaction**: Transfer money between accounts by specifying the source account, destination account, amount, and description of the transaction.

### Additional Configuration

**Environment Variables**:

.env file is used to manage the environment variables for both the backend and frontend applications.

**Frontend Development**: You can run the frontend React app locally with:

```bash

cd app
yarn start
```

**Backend Development**: You can run the backend API locally with:

```bash

cd TaxPayApi
dotnet run
```
