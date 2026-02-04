# AGENTS.md

This file provides guidance for AI coding agents working in this repository.

## Build, Lint, and Test Commands

### Build
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Minimes.Web/Minimes.Web.csproj

# Build with detailed diagnostics
dotnet build -v d
```

### Run Application
```bash
# Run web project (default - SQLite database)
dotnet run --project src/Minimes.Web

# Run with production config (MySQL database)
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/Minimes.Web
```

### Run Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run a single test class
dotnet test --filter "FullyQualifiedName~Minimes.Tests.Domain.*"

# Run a single test method
dotnet test --filter "FullyQualifiedName~Minimes.Tests.Domain.WeighingRecordTests.Should_Create_Valid_Record"

# Run tests in specific project
dotnet test tests/Minimes.Tests/Minimes.Tests.csproj
```

### Database Migrations
```bash
# Add migration (run from Infrastructure directory)
cd src/Minimes.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../Minimes.Web

# Update database
dotnet ef database update --startup-project ../Minimes.Web

# Remove last migration
dotnet ef migrations remove --startup-project ../Minimes.Web
```

---

## Code Style Guidelines

### Project Structure (Clean Architecture)
- **Domain**: Entities, Value Objects, Enums, Repository Interfaces (no dependencies)
- **Application**: Services, DTOs, Validators, Mappings (depends on Domain)
- **Infrastructure**: Repository Implementations, Hardware, Excel Export (depends on Domain + Application)
- **Web**: Blazor Pages, Components, Hubs (depends on all layers)

### Naming Conventions
| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `WeighingRecordService`, `UserResponse` |
| Interfaces | PascalCase with "I" prefix | `IUserService`, `IRepository<T>` |
| Methods | PascalCase | `GetByIdAsync`, `CreateAsync` |
| Properties | PascalCase | `FullName`, `CreatedAt` |
| Private fields | camelCase with `_` prefix | `_repository`, `_logger` |
| Constants | PascalCase | `MaxPasswordLength` |
| Parameters | camelCase | `userId`, `fullName` |
| Local variables | camelCase | `existingUser`, `validationResult` |

### File Organization
- One public class per file (filename matches class name)
- Related DTOs grouped in subdirectories: `DTOs/Customer/`, `DTOs/Product/`
- Services: `Services/` directory
- Validators: `Validators/` directory with parallel structure to DTOs

### Imports
```csharp
// Namespace imports (preferred)
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;

// File-scoped namespace (required)
namespace Minimes.Application.Services;

// No using statements after namespace declaration
```

### Types
- Use `string` instead of `String`
- Use `int`, `bool`, `decimal` instead of `Int32`, `Boolean`, `Decimal`
- Use `Task<T>` for async operations, not `ValueTask<T>` unless justified
- Use nullable reference types (`T?`) for optional values
- Collections: `IEnumerable<T>` for read-only, `ICollection<T>` for modification

### Async/Await
- All I/O operations must be async
- Use `async`/`await` consistently - no `.Result` or `.GetAwaiter().GetResult()`
- Return `Task` or `Task<T>` from async methods
- Always configure `.ConfigureAwait(false)` in library code

### Error Handling
- Use exceptions for exceptional cases, not flow control
- Validate inputs with `ArgumentNullException`, `ArgumentException`
- Business rule violations: `InvalidOperationException`
- Validation failures: `ValidationException` with FluentValidation
- Log errors using `ILogger<T>`
- Never swallow exceptions without logging

### Null Safety
- Enable nullable reference types
- Use null-coalescing (`??`) and null-conditional (`?.`) operators
- Check for null before dereferencing
- Use `default` or `default!` for DI-injected dependencies that can't be null

### Code Patterns
- Constructor injection for dependencies
- Repository pattern with async methods (`*Async` suffix)
- Service layer with interface (`I*Service` / `*Service`)
- AutoMapper for entity-to-DTO mapping
- FluentValidation for input validation

### Razor Components (.razor)
- Use `@L["ResourceKey"]` for all user-visible text (i18n)
- Use `@attribute [Authorize]` for protected pages
- Use `@inject` for DI
- Code-behind: use `@code { }` in the same file (no .razor.cs files)

### Comments
- XML documentation for public APIs (`/// <summary>`)
- No TODO comments - create issues instead
- Explain "why", not "what"
- Remove commented-out code

### Testing
- Use xUnit as test framework
- Use FluentAssertions for assertions
- Use Moq for mocking
- Follow naming: `MethodName_Scenario_ExpectedResult`
- Arrange/Act/Assert structure
- Test one behavior per test
