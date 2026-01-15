---
description: 'Guidelines for writing c# unit/integration tests using xUnit and Moq'
applyTo: '**/*Tests.cs'
---
## Testing

- Always include test cases for critical paths of the application.
- Guide users through creating unit tests.
- Do not emit "Act", "Arrange" or "Assert" comments.
- Copy existing style in nearby files for test method names and capitalization.
- Demonstrate how to mock dependencies using Moq, and strict behaviour for effective testing.
- Use XUnit test library for writing tests