
CREATE DATABASE AromaDomDev;
GO

USE AromaDomDev;
GO

CREATE TABLE MaterialTypes (
    MaterialTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    LossPercent DECIMAL(5,2) NOT NULL
        DEFAULT 0
        CHECK (LossPercent >= 0 AND LossPercent <= 100)
);

CREATE TABLE ProductTypes (
    ProductTypeId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Coefficient DECIMAL(8,2) NOT NULL
        CHECK (Coefficient > 0)
);

CREATE TABLE Materials (
    MaterialId INT IDENTITY(1,1) PRIMARY KEY,
    MaterialTypeId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL UNIQUE,
    Price DECIMAL(12,2) NOT NULL
        CHECK (Price >= 0),
    StockQuantity DECIMAL(12,3) NOT NULL
        CHECK (StockQuantity >= 0),
    MinQuantity DECIMAL(12,3) NOT NULL
        CHECK (MinQuantity >= 0),
    PackageQuantity DECIMAL(12,3) NOT NULL
        CHECK (PackageQuantity > 0),
    Unit NVARCHAR(30) NOT NULL,

    FOREIGN KEY (MaterialTypeId)
        REFERENCES MaterialTypes(MaterialTypeId)
);

CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Article NVARCHAR(30) NOT NULL UNIQUE,
    ProductTypeId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL UNIQUE,
    MinPartnerPrice DECIMAL(12,2) NOT NULL
        CHECK (MinPartnerPrice >= 0),

    FOREIGN KEY (ProductTypeId)
        REFERENCES ProductTypes(ProductTypeId)
);

CREATE TABLE MaterialProducts (
    MaterialId INT NOT NULL,
    ProductId INT NOT NULL,
    MaterialQuantity DECIMAL(12,3) NOT NULL
        CHECK (MaterialQuantity > 0),

    PRIMARY KEY (MaterialId, ProductId),

    FOREIGN KEY (MaterialId)
        REFERENCES Materials(MaterialId),

    FOREIGN KEY (ProductId)
        REFERENCES Products(ProductId)
);
GO



-- Таблицы импорта

USE AromaDomDev;
GO

CREATE TABLE Import_MaterialTypes (
    Name NVARCHAR(200),
    LossPercent NVARCHAR(50)
);

CREATE TABLE Import_ProductTypes (
    Name NVARCHAR(200),
    Coefficient NVARCHAR(50)
);

CREATE TABLE Import_Materials (
    Name NVARCHAR(200),
    MaterialType NVARCHAR(200),
    Price NVARCHAR(50),
    StockQuantity NVARCHAR(50),
    MinQuantity NVARCHAR(50),
    PackageQuantity NVARCHAR(50),
    Unit NVARCHAR(50)
);

CREATE TABLE Import_Products (
    Article NVARCHAR(50),
    ProductType NVARCHAR(200),
    Name NVARCHAR(200),
    MinPartnerPrice NVARCHAR(50)
);

CREATE TABLE Import_MaterialProducts (
    MaterialName NVARCHAR(200),
    ProductName NVARCHAR(200),
    MaterialQuantity NVARCHAR(50)
);
GO


BULK INSERT dbo.Import_MaterialTypes
FROM 'C:\Users\kirend\Desktop\АД_Material_type_import.csv'
with (
	FORMAT = 'CSV',
	CODEPAGE = '65001',
	FIELDTERMINATOR = ';',
    FIELDQUOTE = '"',
    FIRSTROW = 2,
    ROWTERMINATOR = '0x0a'
);

BULK INSERT dbo.Import_Materials
FROM 'C:\Users\kirend\Desktop\АД_Materials_import.csv'
with (
	FORMAT = 'CSV',
	CODEPAGE = '65001',
	FIELDTERMINATOR = ';',
    FIELDQUOTE = '"',
    FIRSTROW = 2,
    ROWTERMINATOR = '0x0a'
);

BULK INSERT dbo.Import_MaterialProducts
FROM 'C:\Users\kirend\Desktop\АД_Material_products_import.csv'
with (
	FORMAT = 'CSV',
	CODEPAGE = '65001',
	FIELDTERMINATOR = ';',
    FIELDQUOTE = '"',
    FIRSTROW = 2,
    ROWTERMINATOR = '0x0a'
);

BULK INSERT dbo.Import_ProductTypes
FROM 'C:\Users\kirend\Desktop\АД_Product_type_import.csv'
with (
	FORMAT = 'CSV',
	CODEPAGE = '65001',
	FIELDTERMINATOR = ';',
    FIELDQUOTE = '"',
    FIRSTROW = 2,
    ROWTERMINATOR = '0x0a'
);




--Очистка данных

--убрать %, заменить , на . и убрать 
SELECT TRIM(REPLACE(REPLACE(REPLACE(LossPercent, '%', ''), ',', '.'), CHAR(13), ''))
from Import_MaterialTypes
--УБРАТЬ CHAR(10) и CHAR(13)


--преобразование
select 
	CAST(
		TRIM(REPLACE(REPLACE(REPLACE(LossPercent, '%', ''), ',', '.'), CHAR(13), ''))
		AS DECIMAL(5,2)
	)
from Import_MaterialTypes
-- или
select 
	CONVERT(
		DECIMAL(5,2),
		TRIM(REPLACE(REPLACE(REPLACE(LossPercent, '%', ''), ',', '.'), CHAR(13), ''))
	)
from Import_MaterialTypes

-- перенос в основную таблицу
insert into MaterialTypes (Name, LossPercent)
select 
	trim(Name),
	TRY_CONVERT(
		DECIMAL(5,2),
		TRIM(REPLACE(REPLACE(REPLACE(LossPercent, '%', ''), ',', '.'), CHAR(13), ''))
	)
FROM Import_MaterialTypes


insert into ProductTypes
select 
	trim(Name),
	CONVERT(
		decimal(5,2),
		TRIM(REPLACE(REPLACE(Coefficient, ',', '.'), CHAR(13), ''))
	)
from Import_ProductTypes

select * from ProductTypes

INSERT INTO Materials (
    MaterialTypeId,
    Name,
    Price,
    StockQuantity,
    MinQuantity,
    PackageQuantity,
    Unit
)
SELECT
    mt.MaterialTypeId,
    TRIM(i.Name),
    TRY_CONVERT(DECIMAL(12,2),
        REPLACE(TRIM(i.Price), ',', '.')),
    TRY_CONVERT(DECIMAL(12,3),
        REPLACE(TRIM(i.StockQuantity), ',', '.')),
    TRY_CONVERT(DECIMAL(12,3),
        REPLACE(TRIM(i.MinQuantity), ',', '.')),
    TRY_CONVERT(DECIMAL(12,3),
        REPLACE(TRIM(i.PackageQuantity), ',', '.')),
    TRIM(i.Unit)
FROM Import_Materials i
JOIN MaterialTypes mt
    ON mt.Name = TRIM(i.MaterialType);


INSERT INTO Products (
    Article,
    ProductTypeId,
    Name,
    MinPartnerPrice
)
SELECT
    TRIM(i.Article),
    pt.ProductTypeId,
    TRIM(i.Name),
    TRY_CONVERT(DECIMAL(12,2),
        REPLACE(REPLACE(TRIM(i.MinPartnerPrice), ',', '.'), CHAR(13), ''))
FROM Import_Products i
JOIN ProductTypes pt
    ON pt.Name = TRIM(i.ProductType);


INSERT INTO MaterialProducts (
    MaterialId,
    ProductId,
    MaterialQuantity
)
SELECT
    m.MaterialId,
    p.ProductId,
    TRY_CONVERT(DECIMAL(12,3),
        REPLACE(REPLACE(TRIM(i.MaterialQuantity), ',', '.'), CHAR(13), ''))
FROM Import_MaterialProducts i
JOIN Materials m
    ON m.Name = TRIM(i.MaterialName)
JOIN Products p
    ON p.Name = TRIM(i.ProductName);


USE AromaDomDev;
GO

DROP TABLE dbo.Import_MaterialProducts
DROP TABLE dbo.Import_Materials
DROP TABLE dbo.Import_Products
DROP TABLE dbo.Import_ProductTypes