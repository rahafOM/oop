# Library Management System

OOP Final Project – Knowledge Academy

## Requirements
- .NET 8 SDK
- Visual Studio or Visual Studio Code

## Run
```bash
dotnet restore
dotnet build
dotnet run
```

## Demo login
- Username: `admin`
- Password: `admin123`

A sample member is created automatically with membership ID `1`.
Members sign in with their membership ID and registered email.

## Features
- Separate librarian and member menus
- Librarian login and member registration
- Books, magazines and research papers
- Search by item ID or title
- Item details editing and activation/deactivation without deleting history
- Member lookup, account history, activation and configurable loan limits
- Borrowing, returning and loan history
- Different late-fee policies by item type
- Partial/full fine payments with unique payment references
- Reports: availability, active loans, popular items, overdue members and loans, unpaid fines and collected payments

## OOP concepts
- **Encapsulation:** private setters and controlled update methods
- **Inheritance:** `Book`, `Magazine`, `ResearchPaper` inherit `LibraryItem`; `Member` and `Librarian` inherit `Person`
- **Abstraction:** `LibraryItem` and `Person` are abstract classes
- **Polymorphism:** `List<LibraryItem>` and overridden `CalculateLateFee()` / `GetLoanPeriod()`
- **Overriding:** item types provide their own late-fee and loan-period implementations
- **Overloading:** `SearchItem(int)` and `SearchItem(string)`
- **Constructors:** validate and initialize objects

## Sample policy
- Book: 14-day loan, 2 currency units per late day
- Magazine: 7-day loan, 1 currency unit per late day
- Research paper: 21-day loan, 3 currency units per late day

These are example policies and can be adjusted to the academy's preferred rules.

## Important limitations
This educational version stores data in memory. Data is reset when the application closes. Passwords are plain text for demonstration only; a real system should use secure password hashing and persistent database/file storage.
