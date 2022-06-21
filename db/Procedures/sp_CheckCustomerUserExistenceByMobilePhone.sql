if exists (select * from sys.objects where name='sp_CheckCustomerUserExistenceByMobilePhone' and type='P')
	drop procedure dbo.sp_CheckCustomerUserExistenceByMobilePhone
GO

create procedure dbo.sp_CheckCustomerUserExistenceByMobilePhone (@MOBILE_PHONE	varchar(20))
AS
	select APPLICATION_USER_ID
	from dbo.CUSTOMER_USER as cu 
	join dbo.APPLICATION_USER as au
		on cu.APPLICATION_USER_ID = au.ID
	where cu.MOBILE_PHONE = @MOBILE_PHONE
GO
