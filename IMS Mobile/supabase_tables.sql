CREATE TABLE cashRegister (
    Id SERIAL PRIMARY KEY,
    CashFlow DOUBLE PRECISION,
    InventoryValue DOUBLE PRECISION,
    TotalSales DOUBLE PRECISION,
    TotalCredit DOUBLE PRECISION
);

CREATE TABLE Contact (
    Id SERIAL PRIMARY KEY,
    Name TEXT,
    PhoneNumber INTEGER,
    CreditScore DOUBLE PRECISION,
    TotalPurchases DOUBLE PRECISION
);

CREATE TABLE Product (
    Id SERIAL PRIMARY KEY,
    Name TEXT,
    Price DOUBLE PRECISION,
    CreatedDate TIMESTAMP DEFAULT NOW(),
    Cost DOUBLE PRECISION,
    stock INTEGER
);

CREATE TABLE Transaction (
    Id SERIAL PRIMARY KEY,
    totalamount DOUBLE PRECISION,
    Type TEXT,
    IsPaid BOOLEAN DEFAULT FALSE,
    CreatedDate TIMESTAMP DEFAULT NOW(),
    ContactId INTEGER
);

CREATE TABLE TransactionProductItem (
    Id SERIAL PRIMARY KEY,
    Name TEXT,
    Price DOUBLE PRECISION,
    Quantity INTEGER DEFAULT 1,
    CategoryName TEXT,
    Cost DOUBLE PRECISION,
    TransactionId INTEGER REFERENCES Transaction(Id),
    ProductId INTEGER REFERENCES Product(Id)
);

CREATE TABLE Entity (
    Id SERIAL PRIMARY KEY
);