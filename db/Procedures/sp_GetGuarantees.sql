if exists (select * from sys.objects where name='sp_GetGuarantees' and type='P')
	drop procedure dbo.sp_GetGuarantees
GO

create procedure sp_GetGuarantees(@ApplicationID uniqueidentifier)
AS
	select  ID,
			CLIENT_CODE,
			AMOUNT,
			CURRENCY
	from GUARANTEE
	where APPLICATION_ID = @ApplicationID
GO
