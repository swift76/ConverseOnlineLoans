if exists (select * from sys.objects where name='sp_GetPledge' and type='P')
	drop procedure dbo.sp_GetPledge
GO

create procedure sp_GetPledge(@ID int)
AS
	select  APPLICATION_ID,
			CLIENT_CODE,
			AMOUNT,
			CURRENCY,
			DETAILS,
			RATIO,
			PLEDGE_NAME,
			PRICE
	from PLEDGE
	where ID = @ID
GO
