DECLARE @SqlStatement VARCHAR(MAX)
SELECT @SqlStatement = 
    COALESCE(@SqlStatement, '') + 'DROP TABLE [SmartFactory].' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'SmartFactory'

PRINT @SqlStatement