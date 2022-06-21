if exists (select * from sys.objects where name='sp_GetCustomerMissingData' and type='P')
	drop procedure dbo.sp_GetCustomerMissingData
GO

create procedure dbo.sp_GetCustomerMissingData(@CUSTOMER_USER_ID	int)
AS
	select  MOBILE_PHONE as MOBILE_PHONE_1,
			EMAIL
	from dbo.CUSTOMER_USER
	where APPLICATION_USER_ID = @CUSTOMER_USER_ID
GO
