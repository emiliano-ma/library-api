# Library API 

### Run the app

    dotnet run

### Run the tests

    dotnet test



### Base URI on LocalHost

https://localhost:5001/

## Books

#### Get list of Books

// GET: api/Books - api/books?author={author} - api/books?title={title}

#### Get a specific book

 // GET: api/Books/{id}

#### Create a book

 // POST: api/Books

Request body - JSON formatted. 
{
    "title": "Title 1",
    "author": "author1",
    "available": true
}

#### Update a book

 // PUT: api/Books/{id}

 {
    "bookId": 4,
    "title": "Title 2",
    "author": "author2",
    "available": false,
    "readerId": 2
}

#### Update/Create a book - Depending on the `bookId` presence/absence in the request.

 // PATCH: api/Books/save

 {
    "bookId": 5,
    "title": "Title 555",
    "author": "author2",
    "available": true
}

#### Delete a book

   // DELETE: api/Books/{id}


#### Response body - JSON formatted. 

{
    "bookId": 5,
    "title": "Title 1",
    "author": "author1",
    "available": true,
    "updatedAt": "2020-12-04T21:17:21.530366+01:00",
    "readerId": 0
}

## Readers

#### Get list of Readers

// GET: api/Readers - api/Readers?name={name} - api/Readers?email={reader@mail.com}

#### Get a specific reader

 // GET: api/Readers/{id}

#### Create a reader

 // POST: api/Readers

Request body - JSON formatted. 
{
    "name": "Firstname Lastname",
    "email": "reader@mail.com"
}

#### Update a reader

 // PUT: api/Readers/{id}

{
    "readerId": 1,
    "name": "Firstname Lastname",
    "email": "reader@mail.com"
}

#### Update/Create a reader - Depending on the `readerId` presence/absence in the request.

 // PATCH: api/Readers/save

{
    "readerId": 1,
    "name": "Firstname Lastname",
    "email": "reader@mail.com"
}

#### Delete a reader

   // DELETE: api/Readers/{id}


#### Response body - JSON formatted. 

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
          "title": "Title 2",
          "author": "author2",
          "available": false,
          "updatedAt": "2020-12-04T22:14:54.693325+01:00",
          "readerId": 2
        }
    ]
  }
]