---
description: 'Generic code review instructions for C# 12, .NET 8, and ASP.NET Core 8 projects.'
---

PERFORM CODE REVIEW of the files from the context.

# Generic Code Review Instructions

Comprehensive code review guidelines for GitHub Copilot that can be adapted to any project. These instructions follow best practices from prompt engineering and provide a structured approach to code quality, security, testing, and architecture review.

## Review Language

When performing a code review, respond in **English**.

## Review Priorities

When performing a code review, prioritize issues in the following order:

### 🔴 CRITICAL (Block merge)
- **Security**: Vulnerabilities, exposed secrets, authentication/authorization issues
- **Correctness**: Logic errors, data corruption risks, race conditions
- **Breaking Changes**: API contract changes without versioning
- **Data Loss**: Risk of data loss or corruption

### 🟡 IMPORTANT (Requires discussion)
- **Code Quality**: Severe violations of SOLID principles, excessive duplication
- **Test Coverage**: Missing tests for critical paths or new functionality
- **Performance**: Obvious performance bottlenecks (N+1 queries, memory leaks)
- **Architecture**: Significant deviations from established patterns

### 🟢 SUGGESTION (Non-blocking improvements)
- **Readability**: Poor naming, complex logic that could be simplified
- **Optimization**: Performance improvements without functional impact
- **Best Practices**: Minor deviations from conventions
- **Documentation**: Missing or incomplete comments/documentation

## General Review Principles

When performing a code review, follow these principles:

1. **Be specific**: Reference exact lines, files, and provide concrete examples
2. **Provide context**: Explain WHY something is an issue and the potential impact
3. **Suggest solutions**: Show corrected code when applicable, not just what's wrong
4. **Be constructive**: Focus on improving the code, not criticizing the author
5. **Recognize good practices**: Acknowledge well-written code and smart solutions
6. **Be pragmatic**: Not every suggestion needs immediate implementation
7. **Group related comments**: Avoid multiple comments about the same topic

## Code Quality Standards

When performing a code review, check for:

### Clean Code
- Descriptive and meaningful names for variables, functions, and classes
- Single Responsibility Principle: each function/class does one thing well
- DRY (Don't Repeat Yourself): no code duplication
- Functions should be small and focused (ideally < 20-30 lines)
- Avoid deeply nested code (max 3-4 levels)
- Avoid magic numbers and strings (use constants)
- Code should be self-documenting; comments only when necessary

#### Examples
```csharp
// ✅ GOOD: Clear naming and constants
public static class DiscountCalculator
{
    private const decimal c_premiumThreshold = 100m;
    private const decimal c_remiumDiscountRate = 0.15m;
    private const decimal c_standardDiscountRate = 0.10m;

    public static decimal CalculateDiscount(decimal orderTotal, decimal itemPrice)
    {
        bool isPremiumOrder = orderTotal > PremiumThreshold;
        decimal discountRate = isPremiumOrder
            ? PremiumDiscountRate
            : StandardDiscountRate;

        return itemPrice * discountRate;
    }
}

```

### Error Handling
- Proper error handling at appropriate levels
- Meaningful error messages
- No silent failures or ignored exceptions
- Fail fast: validate inputs early
- Use appropriate error types/exceptions

#### Examples
```csharp
// ✅ GOOD: Explicit error handling
public void ProcessUser(int userId)
{
    if (userId <= 0)
        throw new ArgumentException($"Invalid userId: {userId}", nameof(userId));

    User user;
    try
    {
        user = db.Get(userId);
    }
    catch (UserNotFoundException)
    {
        throw new UserNotFoundException($"User {userId} not found in database");
    }
    catch (DatabaseException ex)
    {
        throw new ProcessingException($"Failed to retrieve user {userId}", ex);
    }

    user.Process();
}
```
## Testing Standards

When performing a code review, verify test quality:

- **Coverage**: Critical paths and new functionality must have tests
- **Test Names**: Descriptive names that explain what is being tested
- **Test Structure**: Clear Arrange-Act-Assert or Given-When-Then pattern
- **Independence**: Tests should not depend on each other or external state
- **Assertions**: Use specific assertions, avoid generic assertTrue/assertFalse
- **Edge Cases**: Test boundary conditions, null values, empty collections
- **Mock Appropriately**: Mock external dependencies, not domain logic

#### Examples
```csharp
[Fact]
public void Calculate10PercentDiscountForOrdersUnder100()
{
    // Arrange
    decimal orderTotal = 50m;
    decimal itemPrice = 20m;

    // Act
    decimal discount = CalculateDiscount(orderTotal, itemPrice);

    // Assert
    Assert.Equal(2.00m, discount);
}

```

## Performance Considerations

When performing a code review, check for performance issues:

- **Database Queries**: Avoid N+1 queries, use proper indexing
- **Algorithms**: Appropriate time/space complexity for the use case
- **Caching**: Utilize caching for expensive or repeated operations
- **Resource Management**: Proper cleanup of connections, files, streams
- **Pagination**: Large result sets should be paginated
- **Lazy Loading**: Load data only when needed

#### Examples
```csharp
var users = dbContext.Users
    .Include(u => u.Orders)
    .ToList();

foreach (var user in users)
{
    var orders = user.Orders; // already loaded ✅
}
```

## Architecture and Design

When performing a code review, verify architectural principles:

- **Separation of Concerns**: Clear boundaries between layers/modules
- **Dependency Direction**: High-level modules don't depend on low-level details
- **Interface Segregation**: Prefer small, focused interfaces
- **Loose Coupling**: Components should be independently testable
- **High Cohesion**: Related functionality grouped together
- **Consistent Patterns**: Follow established patterns in the codebase

## Comment Format Template

When performing a code review, use this format for comments:

```markdown
**[PRIORITY] Category: Brief title**

Reviewed file path: "\directory\file.ext"
Position of the issue in the file: [Line Number:Character Number].

Detailed description of the issue or suggestion.

**Why this matters:**
Explanation of the impact or reason for the suggestion.

**Suggested fix:**
[code example if applicable]

**Reference:** [link to relevant documentation or standard]
```

### Example Comments

#### Critical Issue
```markdown
**🔴 CRITICAL - Security: SQL Injection Vulnerability**

Reviewed file path: "\main\Zerto.PromotionWorker.Services\HostedServices\JournalCacheCleanupHostedService.cs"
Position of the issue in the file: [232:13].

The query on line 45 concatenates user input directly into the SQL string,
creating a SQL injection vulnerability.

**Why this matters:**
An attacker could manipulate the email parameter to execute arbitrary SQL commands,
potentially exposing or deleting all database data.

**Suggested fix:**
```sql
PreparedStatement stmt = conn.prepareStatement(
    "SELECT * FROM users WHERE email = ?"
);
stmt.setString(1, email);
```

**Reference:** OWASP SQL Injection Prevention Cheat Sheet
```

#### Important Issue
```markdown
**🟡 IMPORTANT - Testing: Missing test coverage for critical path**
Reviewed file path: "\main\Zerto.PromotionWorker.Services\HostedServices\JournalCacheCleanupHostedService.cs"
Position of the issue in the file: [232:13].

The `processPayment()` function handles financial transactions but has no tests
for the refund scenario.

**Why this matters:**
Refunds involve money movement and should be thoroughly tested to prevent
financial errors or data inconsistencies.

**Suggested fix:**
Add test case:
```csharp
[Fact]
public void Should_Process_Full_Refund_When_Order_Is_Cancelled()
{
    // Arrange
    var order = CreateOrder(new Order
    {
        Total = 100m,
        Status = OrderStatus.Cancelled
    });

    var paymentRequest = new PaymentRequest
    {
        Type = PaymentType.Refund
    };

    // Act
    var result = ProcessPayment(order, paymentRequest);

    // Assert
    Assert.Equal(100m, result.RefundAmount);
    Assert.Equal(PaymentStatus.Refunded, result.Status);
}

```
```

#### Suggestion
```markdown
**🟢 SUGGESTION - Readability: Simplify nested conditionals**

Reviewed file path: "\main\Zerto.PromotionWorker.Services\HostedServices\JournalCacheCleanupHostedService.cs"
Position of the issue in the file: [232:13].

The nested if statements on lines 30-40 make the logic hard to follow.

**Why this matters:**
Simpler code is easier to maintain, debug, and test.

**Suggested fix:**
```charp
if (user is null || !user.IsActive || !user.HasPermission("write"))
{
    return;
}

// do something

```


## Review Checklist

When performing a code review, systematically verify:

### Code Quality
- [ ] Code follows consistent style and conventions
- [ ] Names are descriptive and follow naming conventions
- [ ] Functions/methods are small and focused
- [ ] No code duplication
- [ ] Complex logic is broken into simpler parts
- [ ] Error handling is appropriate
- [ ] No commented-out code or TODO without tickets

### Security
- [ ] No sensitive data in code or logs
- [ ] Input validation on all user inputs
- [ ] No SQL injection vulnerabilities
- [ ] Authentication and authorization properly implemented
- [ ] Dependencies are up-to-date and secure

### Testing
- [ ] New code has appropriate test coverage
- [ ] Tests are well-named and focused
- [ ] Tests cover edge cases and error scenarios
- [ ] Tests are independent and deterministic
- [ ] No tests that always pass or are commented out

### Performance
- [ ] No obvious performance issues (N+1, memory leaks)
- [ ] Appropriate use of caching
- [ ] Efficient algorithms and data structures
- [ ] Proper resource cleanup
- [ ] (Race Condition / TOCTOU): Verify no time-of-check/time-of-use gaps—especially around cache population/eviction, file existence + replace/delete, and shared state. Ensure atomic operations, proper locking, and concurrency-safe patterns are used.

### Architecture
- [ ] Follows established patterns and conventions
- [ ] Proper separation of concerns
- [ ] No architectural violations
- [ ] Dependencies flow in correct direction
