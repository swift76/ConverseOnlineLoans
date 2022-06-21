if exists (select * from sys.objects where name='sp_CheckCustomerUserExistenceByEmail' and type='P')
	drop procedure dbo.sp_CheckCustomerUserExistenceByEmail
GO

create procedure dbo.sp_CheckCustomerUserExistenceByEmail (@EMAIL	varchar(70))
AS
	select APPLICATION_USER_ID
	from dbo.CUSTOMER_USER as cu 
	join dbo.APPLICATION_USER as au
		on cu.APPLICATION_USER_ID = au.ID
	where cu.EMAIL = @EMAIL
GO
