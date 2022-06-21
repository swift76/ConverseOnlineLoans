if exists (select * from sys.objects where name='sp_GetPledges' and type='P')
	drop procedure dbo.sp_GetPledges
GO

create procedure sp_GetPledges(@ApplicationID uniqueidentifier)
AS
	select  ID,
			CLIENT_CODE,
			AMOUNT,
			CURRENCY,
			DETAILS,
			RATIO,
			PLEDGE_NAME,
			PRICE
	from PLEDGE
	where APPLICATION_ID = @ApplicationID
GO
