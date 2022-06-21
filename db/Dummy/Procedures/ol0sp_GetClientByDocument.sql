if exists (select * from sysobjects where id = object_id('dbo.ol0sp_GetClientByDocument') and sysstat & 0xf = 4)
	drop procedure dbo.ol0sp_GetClientByDocument
GO

CREATE PROCEDURE ol0sp_GetClientByDocument(@PassportCode	varchar(20),
										   @PassportType	char(2),
										   @SocialCardCode	varchar(20))
AS
	select '12345678' as ClientCode
GO
