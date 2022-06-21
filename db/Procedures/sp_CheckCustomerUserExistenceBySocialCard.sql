if exists (select * from sys.objects where name='sp_CheckCustomerUserExistenceBySocialCard' and type='P')
	drop procedure dbo.sp_CheckCustomerUserExistenceBySocialCard
GO

create procedure dbo.sp_CheckCustomerUserExistenceBySocialCard (@SOCIAL_CARD_NUMBER	char(10))
AS
	select APPLICATION_USER_ID
	from dbo.CUSTOMER_USER as cu
	join dbo.APPLICATION_USER as au
		on cu.APPLICATION_USER_ID = au.ID
	where cu.SOCIAL_CARD_NUMBER = @SOCIAL_CARD_NUMBER
GO
