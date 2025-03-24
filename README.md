## 🔍 Swagger API Explorer

Explore a RESTful interface for the Developer Evaluation API via Swagger Redoc. The interface categorizes endpoints for clarity, offering grouped access to core business operations like user management, sales tracking, product listing, branch registration, and authentication.

### 🛍️ General API Navigation

The Swagger UI provides a sidebar menu where endpoints are grouped by domain context. Each group exposes its available HTTP methods (GET, POST, DELETE), expected parameters, and sample responses.

📸 *Overview of Swagger UI with endpoint groups*  
![Overview of Swagger UI with endpoint groups (Users, Sales, Products, Branches, Auth)](https://github.com/user-attachments/assets/27c082c1-7112-4c70-a7d9-2a1a0f322682)

![image]

---

### 🔐 Authentication

Handles token-based user authentication via credential submission.

📸 *Authentication endpoint using POST method*  
![image](https://github.com/user-attachments/assets/055a0360-b844-4123-a896-044eedd5b92f)

![Authentication endpoint using POST method](https://github.com/user-attachments/assets/a6c93c7f-1133-4d3d-8f9f-1973745c25bf)

![image](https://github.com/user-attachments/assets/c033f035-381e-4062-985b-22d656d83db9)



- **POST /api/auth/login** – Authenticates a user and returns a JWT token if credentials are valid.

---

### 👤 Users

This group manages user-related operations such as account creation, retrieval, listing, and deletion.

📸 *User endpoints and their responses in Swagger UI*  
![User endpoints and their responses in Swagger UI](https://github.com/user-attachments/assets/ad7e5745-5ace-45ac-82c1-6c42d86ab4bf)

![image](https://github.com/user-attachments/assets/c416ac6b-9f0d-415c-b26a-86668f9258c9)



- **POST /api/users** – Registers a new user with username, email, and password.
- **GET /api/users/{id}** – Retrieves details of a specific user by ID.
- **GET /api/users** – Lists all users with optional pagination.
- **DELETE /api/users/{id}** – Deletes a user by ID.

Each route includes validation error handling with structured responses for 400/404 cases.

---

### 💸 Sales

Manages sales records, including creation, retrieval, and cancellation. Applies quantity-based discount business rules automatically during sale creation.

📸 *Sales endpoints with HTTP verbs*  
![Sales endpoints with HTTP verbs](https://github.com/user-attachments/assets/058429ce-e7f6-4239-860f-ed187653cefb)

![image](https://github.com/user-attachments/assets/323c267b-adc7-4512-b81e-1ac79c6e0454)

- **POST /api/sales** – Creates a new sale including items, quantity, unit price, and applies automatic discounts.
- **GET /api/sales/{id}** – Retrieves sale details by ID.
- **DELETE /api/sales/{id}** – Cancels a specific sale.

⚠️ Discount rules:
- 4+ units: 10% off
- 10–20 units: 20% off
- >20 units: **Invalid**

---

### 🏢 Branches

Controls branch registration and management within the platform.

📸 *Branch endpoints listed in Swagger UI*  
![Branch endpoints listed in Swagger UI](https://github.com/user-attachments/assets/fb9cab32-a2c4-4803-9800-e9d974e4dd41)

![image](https://github.com/user-attachments/assets/766080c2-287d-4198-9571-1e0f4c68a7a0)



- **POST /api/branches** – Creates a new branch.
- **GET /api/branches/{id}** – Fetches a branch by ID.
- **DELETE /api/branches/{id}** – Deletes a branch.

All endpoints return structured `ApiResponse` or `ApiResponseWithData`, including success flags and messages.

---

### 📦 Products
![image](https://github.com/user-attachments/assets/e7c93962-206d-4783-b11c-06f08e3bf968)

![image](https://github.com/user-attachments/assets/0e97f4cb-f9cd-4d4e-9f7e-e76659943b4d)

Manages the product catalog and supports listing, creation, and removal.

- **POST /api/products** – Adds a product with name, description, and unit price.
- **GET /api/products/{id}** – Fetches a product by ID.
- **DELETE /api/products/{id}** – Deletes a product.

Follows the same validation pattern as branches and users.

---

### 📊 Logging & Observability

The application uses **Serilog + Seq** for structured logging of key events such as creation, deletion, and retrieval of entities. This improves auditability and debugging visibility during development and production.

📸 *Logging events using Seq for auditing and debugging*  
![Logging events using Seq for auditing and debugging](https://github.com/user-attachments/assets/e01d30eb-f237-43af-8710-8142f5c0301d)

## Serilog Seq Queries
![image](https://github.com/user-attachments/assets/49b1b68e-513a-4889-ac7a-f7603f41f6e8)
![image](https://github.com/user-attachments/assets/0774d56b-39e8-4b09-b4b4-50804bf452ef)


## Postman collection 
![image](https://github.com/user-attachments/assets/31aa2bf6-56bf-4875-beb5-61b36e88fded)


Example logs:
```text
BranchCreatedEvent { BranchId = ..., Name = "BranchToDelete" }
SaleCreatedEvent { SaleId = ..., Total = 199.99 }
ProductDeletedEvent { ProductId = ..., Name = "Product X" }
´´´text
