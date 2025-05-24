# Fermion.Domain.Extensions

Fermion.Extensions is a library containing helper extension methods for use in the domain layer.

## Features

- Collection extensions
- String extensions
- DateTime extensions
- Object extensions
- LINQ extensions
- HTTP Context extensions
- Service Collection extensions
- File extensions
- JSON extensions
- Claims extensions

## Installation

``` bash
   dotnet add package Fermion.Extensions
```

## Content

### Collection Extensions
- `AddRange<T>`: Adds multiple items to collection
- `RemoveRange<T>`: Removes multiple items from collection
- `IsNullOrEmpty<T>`: Checks if collection is null or empty
- `HasItems<T>`: Checks if collection has any items
- `DistinctBy<T>`: Returns distinct elements by specified property
- `ForEach<T>`: Performs action on each element

### String Extensions
- `IsNullOrEmpty`: Checks if string is null or empty
- `HasValue`: Checks if string has value
- `ToTitleCase`: Converts string to title case
- `ToSlug`: Converts string to URL-friendly slug
- `Truncate`: Truncates string to specified length
- `RemoveAccents`: Removes diacritics from string
- `ToBase64`: Converts string to Base64
- `FromBase64`: Converts Base64 to string

### DateTime Extensions
- `ToFormattedString`: Formats date to "dd.MM.yyyy HH:mm:ss"
- `IsBetween`: Checks if date is between two dates
- `StartOfDay`: Gets start of day
- `EndOfDay`: Gets end of day
- `StartOfWeek`: Gets start of week
- `EndOfWeek`: Gets end of week
- `StartOfMonth`: Gets start of month
- `EndOfMonth`: Gets end of month

### Object Extensions
- `ToJson`: Converts object to JSON string
- `FromJson<T>`: Converts JSON string to object
- `Clone<T>`: Creates deep copy of object
- `IsNull`: Checks if object is null
- `IsNotNull`: Checks if object is not null

### LINQ Extensions
- `WhereIf`: Conditionally applies Where clause
- `OrderByIf`: Conditionally applies OrderBy clause
- `SkipIf`: Conditionally applies Skip clause
- `TakeIf`: Conditionally applies Take clause

### HTTP Context Extensions
- `GetClientIP`: Gets client IP address
- `GetUserAgent`: Gets user agent string
- `GetDeviceInfo`: Gets device information
- `GetRequestPath`: Gets request path
- `GetRequestMethod`: Gets request method

### Service Collection Extensions
- `AddServices`: Adds services with lifetime
- `AddRepositories`: Adds repositories with lifetime
- `AddValidators`: Adds validators with lifetime
- `AddMappers`: Adds mappers with lifetime

### File Extensions
- `GetFileExtension`: Gets file extension
- `GetFileName`: Gets file name
- `GetFileSize`: Gets file size
- `IsImage`: Checks if file is image
- `IsPdf`: Checks if file is PDF
- `IsExcel`: Checks if file is Excel

### JSON Extensions
- `MaskSensitiveData`: Masks sensitive data in JSON
- `UnmaskSensitiveData`: Unmasks sensitive data in JSON
- `ToJson`: Converts object to JSON string
- `FromJson<T>`: Converts JSON string to object

### Claims Extensions
- `GetUserId`: Gets user ID from claims
- `GetUserName`: Gets username from claims
- `GetUserEmail`: Gets user email from claims
- `GetUserRoles`: Gets user roles from claims
- `HasRole`: Checks if user has role
- `HasClaim`: Checks if user has claim

## Features

- Null-safe operations
- Type conversion helpers
- Collection manipulation
- Date/time utilities
- HTTP context utilities
- File handling
- JSON processing
- Claims processing