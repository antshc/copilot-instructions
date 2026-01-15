# General Code Review Standards

## Purpose

These instructions guide Copilot code review across all files in this repository.
Language-specific rules are in separate instruction files.

## Security Critical Issues

- Check for hardcoded secrets, API keys, or credentials
- Verify proper input validation and sanitization
- Review authentication and authorization logic

## Performance Red Flags

- Identify N+1 database query problems
- Spot inefficient loops and algorithmic issues
- Check for memory leaks and resource cleanup
- Review caching opportunities for expensive operations

## Code Quality Essentials

- Functions should be focused and appropriately sized (under 50 lines)
- Use clear, descriptive naming conventions
- Ensure proper error handling throughout
- Remove dead code and unused imports

## Review Style

- Acknowledge good patterns when you see them
- Focus on significant issues rather than minor stylistic preferences
- Consider the broader context of the codebase
- MAKE ONLY **high confidence** suggestions
- Be specific, Provide actionable suggestions and explain your reasoning.


## Testing Standards

- New features require unit tests
- Tests should cover edge cases and error conditions
- Test names should clearly describe what they test

Always prioritize security vulnerabilities and performance issues that could impact users.
