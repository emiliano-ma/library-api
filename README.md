# Library API 

### Run the app

    dotnet run

### Run the tests

    dotnet test

### Database:

    Managed with Entity Framework Core ORM.

    Configured to work with the following Database providers:

    * In-Memory Database Provider
    * SQLite 
    * SQL Server (Running on Docker)

    The code for the different configurations is commented out for easy DB interchange while on Development.



### Base URI on LocalHost

    https://localhost:5001/

### Features

    * Logs - Using NLog.

-------
## Endpoints for Books:

### Get list of Books

    GET: api/Books (all books)
    GET: api/books?author={author} (by Author)
    GET: api/books?title={title} (by Title)

### Get a specific book

    GET: api/Books/{id}

### Create a book

    POST: api/Books

    {
        "title": "Title 1",
        "author": "Author 1",
        "available": true
    }

### Update a book

    PUT: api/Books/{id}

    {
        "bookId": 4,
        "title": "Title 2",
        "author": "Author 2",
        "available": false,
        "readerId": 2
    }

### Update/Create a book - Depending on the `bookId` presence/absence in the request.

    PATCH: api/Books/save

    {
        "bookId": 5,
        "title": "Title 555",
        "author": "Author 2",
        "available": true
    }

### Update/Create a books in a batch - Depending on the `Title` matching an existing book.

    PATCH: api/Books/save/batch

    [
        {
            "title": "Title 555",
            "author": "Author 2",
            "available": false
        },
        {
            "title": "Title 5",
            "author": "Author 2",
            "available": true
        }
    ]

### Delete a book

    DELETE: api/Books/{id}


### Response body for single Book: 

    {
        "bookId": 5,
        "title": "Title 1",
        "author": "Author 1",
        "available": true,
        "updatedAt": "2020-12-04T21:17:21.530366+01:00",
        "readerId": 0
    }

-------
## Endpoints for Readers

### Get list of Readers

    GET: api/Readers (all readers)
    GET: api/Readers?name={name} (by Name)
    GET: api/Readers?email={reader@mail.com} (by Email)

### Get a specific reader

    GET: api/Readers/{id}

### Create a reader

    POST: api/Readers

    {
        "name": "Firstname Lastname",
        "email": "reader@mail.com"
    }

### Update a reader

    PUT: api/Readers/{id}

    {
        "readerId": 1,
        "name": "Firstname Lastname",
        "email": "reader22@mail.com"
    }

### Update/Create a reader - Depending on the `readerId` presence/absence in the request.

    PATCH: api/Readers/save

    {
        "readerId": 1,
        "name": "Firstname Lastname",
        "email": "reader44@mail.com"
    }

### Delete a reader

    DELETE: api/Readers/{id}


### Response body for Readers List and associated books: 

    [
        {
            "readerId": 1,
            "name": "Firstname Lastname",
            "email": "reader@mail.com",
            "books": []
        },
        {
            "readerId": 2,
            "name": "Emiliano",
            "email": "emiliano@mail.com",
            "books": [
                {
                "bookId": 4,
                "title": "Title 25",
                "author": "Author 2",
                "available": false,
                "updatedAt": "2020-12-04T22:14:54.693325+01:00",
                "readerId": 2
                }
            ]
        }
    ]

*-Response body / Request body- in this document are JSON formatted for quick use on Postman (or similar).*