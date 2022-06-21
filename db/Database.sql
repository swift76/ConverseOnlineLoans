SP_CONFIGURE 'show advanced options', 1
GO
RECONFIGURE
GO
sp_configure 'clr enabled', 1
GO
RECONFIGURE
GO

USE master
GO

CREATE DATABASE $(DatabaseName)
GO
USE $(DatabaseName)
GO

ALTER DATABASE $(DatabaseName) SET TRUSTWORTHY ON
GO

--ALTER AUTHORIZATION ON DATABASE::$(DatabaseName) TO [sa]
--GO
