if exists (select * from sys.objects where name='sp_GetCustomerStatusID' and type='P')
	drop procedure dbo.sp_GetCustomerStatusID
GO

create procedure dbo.sp_GetCustomerStatusID(@ID	uniqueidentifier)
AS
	select CUSTOMER_STATUS_ID
	from dbo.APPLICATION
	where ID = @ID
GO
