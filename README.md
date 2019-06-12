# Discount

Discount is an open source proof of concept, for a gamified application that enables users to learn better shopping habits.

### Setup guide

  - This application requires you to setup a hosted Azure SQL solution

First you need to locate the database connection controller:
```
Database/ConnectionController.cs
```

Scroll down to line 55 and edit the following lines
```cs
DataSource = "ENTER YOUR DATABASE LINK REFERENCE",
UserID = "ENTER YOUR USERNAME FOR THE CONNECTION",
Password = "ENTER YOUR PASSWORD FOR THE CONNECTION",
InitialCatalog = "ENTER THE DATABASENAME"
```

Import the sql database structure to your Azure SQL database.

```sql
CREATE TABLE Shop
(
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(max) NOT NULL,
    Shopgun_dealer_ID VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE Offer 
(
    ID INT PRIMARY KEY IDENTITY(1,1),
    Title VARCHAR(max) NOT NULL,
    Price_before INT NOT NULL,
    Price_offer INT NOT NULL,
    Offer_from DATETIME NOT NULL,
    Offer_to DATETIME NOT NULL,
    Shopgun_offer_ID VARCHAR(64) NOT NULL UNIQUE,
    Shopgun_dealer_id VARCHAR(64) NOT NULL,
    Shopgun_image_url VARCHAR(max) NOT NULL,
    Shop_id INT NOT NULL REFERENCES Shop(ID)
);

CREATE TABLE [dbo].[User]
(
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(max) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Level INT NOT NULL,
    Experience INT NOT NULL
);

CREATE TABLE Purchase
(
    ID INT PRIMARY KEY IDENTITY(1,1),
    Price_purchased INT NOT NULL,
    Purchase_time DATETIME NOT NULL,
    User_id INT NOT NULL REFERENCES [dbo].[User](ID),
    Offer_id INT NOT NULL REFERENCES Offer(ID),
    Shop_id INT NOT NULL REFERENCES Shop(ID)
);
```

Compile, exectute & enjoy!

License
----

MIT

Copyright 2019 Jannick Feifer Ingemann Jørgensen, Jonatan Vestergaard Skovby, Kristoffer Alexander Fich, Troels Dion Jensen, Michael Cavaleri

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

**Free Software, Hell Yeah!**
