IF DB_ID(N'CatalogOfProducts') IS NULL
BEGIN
    PRINT 'Creating database CatalogOfProducts...';
    CREATE DATABASE [CatalogOfProducts];
END
ELSE
BEGIN
    PRINT 'Database CatalogOfProducts already exists.';
END
GO
