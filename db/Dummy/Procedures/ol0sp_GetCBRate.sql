if exists (select * from sys.objects where name='ol0sp_GetCBRate' and type='P')
	drop procedure dbo.ol0sp_GetCBRate
GO

create procedure ol0sp_GetCBRate(@CURRENCY	char(3))
AS
	declare @RESULT money=1
	select @RESULT as VALUE
GO
