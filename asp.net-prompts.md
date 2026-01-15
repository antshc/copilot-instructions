
# 💡 GitHub Copilot Tips for ASP.NET Core C# (Repository Pattern & REST API)

## ✅ Tips to Make Code Generation More Effective

1. **Be Descriptive in Comments or Prompts**  
   Start with a clear, concise comment that tells Copilot what you want. Mention specific patterns (e.g., Repository, Unit of Work) or technologies (e.g., Entity Framework, ASP.NET Core).

2. **Use Naming Conventions Consistently**  
   Copilot learns from the names you use. Stick with consistent naming across files, services, and methods.

3. **Write the Method Signature First**  
   Typing out the method signature (with parameters and return type) gives Copilot a better idea of your intent.

4. **Use Region Blocks and XML Comments**  
   Makes your code more readable and gives Copilot helpful context.

5. **Leverage File Names and Folder Structures**  
   Use meaningful file names like `UserRepository.cs` or `ProductController.cs`. Copilot uses this context for better predictions.

---

## 📌 Prompt Templates for ASP.NET C# (Repository Pattern)

Use the following prompt examples in comments or as file starters:

### 🔹 Repository Interface Prompt
```csharp
// Define a generic repository interface for basic CRUD operations using Entity Framework Core
```

### 🔹 Repository Implementation Prompt
```csharp
// Implement the IGenericRepository interface for the Product entity using DbContext
```

### 🔹 Service Layer Prompt
```csharp
// Create a ProductService class that uses IProductRepository to perform CRUD operations
```

### 🔹 API Controller Prompt
```csharp
// Create a REST API controller for the Product entity with GET, POST, PUT, DELETE methods using dependency injection
```

### 🔹 DTO Conversion Prompt
```csharp
// Create a ProductDto class and map it from/to the Product entity using AutoMapper
```

### 🔹 Unit of Work Pattern Prompt
```csharp
// Implement a UnitOfWork class to manage multiple repositories with a shared DbContext
```

### 🔹 Exception Handling Middleware Prompt
```csharp
// Create a global exception handling middleware in ASP.NET Core to return consistent error responses
```

### 🔹 Pagination Helper Prompt
```csharp
// Create a helper method to return paginated results from an IQueryable<Product> with page number and page size
```

---

## 🚀 Full Prompt Example: Product API Controller

```csharp
// Create an ASP.NET Core REST API controller for managing products using the repository pattern.
// Include endpoints for GetAllProducts, GetProductById, CreateProduct, UpdateProduct, and DeleteProduct.
// Use dependency injection to inject IProductRepository.
// Return appropriate status codes and handle null values.
```

```
// Create a C# class named Product that will be used as an Entity Framework Core model.
// Apply data annotations for validation and schema definition.
// Include properties: Id (int), Name (string), Description (string?), Price (decimal), CreatedAt (DateTime).
// Use PascalCase naming convention. Mark Id as the primary key with [Key].
// Make Name required with a maximum length of 100 characters.
// Set Price with [Range] validation (0.01 to 10000).
// Set CreatedAt to default to current time.
// Use nullable reference types and immutability where possible.
// Add XML summary comments for each property.

```

```
// Create a C# class named Product that will be used as an Entity Framework Core model.
// Include properties: Id (int), Name (string), Description (string?), Price (decimal), CreatedAt (DateTime).
// Use PascalCase naming convention. Use nullable reference types where appropriate.
// Create a separate EF Core configuration class ProductConfiguration that implements IEntityTypeConfiguration<Product>.
// In the configuration class, use Fluent API (ModelBuilder) to:
// - Set Id as the primary key
// - Set Name as required with max length 100
// - Set Description as optional
// - Set Price with range validation (0.01 to 10000) — if range not possible via Fluent API, leave comment
// - Set CreatedAt to have default value of current UTC time
// Use XML summary comments in the model class.

```

