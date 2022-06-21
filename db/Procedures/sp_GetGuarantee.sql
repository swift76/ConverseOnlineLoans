if exists (select * from sys.objects where name='sp_GetGuarantee' and type='P')
	drop procedure dbo.sp_GetGuarantee
GO

create procedure sp_GetGuarantee(@ID int)
AS
	select  APPLICATION_ID,
			CLIENT_CODE,
			AMOUNT,
			CURRENCY
	from GUARANTEE
	where ID = @ID
GO
