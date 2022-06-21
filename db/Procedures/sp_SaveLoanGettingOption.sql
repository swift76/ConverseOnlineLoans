if exists (select * from sys.objects where name='sp_SaveLoanGettingOption' and type='P')
	drop procedure dbo.sp_SaveLoanGettingOption
GO

create procedure dbo.sp_SaveLoanGettingOption(@CODE		char(3),
											  @NAME_AM	nvarchar(50),
											  @NAME_EN	varchar(50))
AS
	insert into dbo.LOAN_GETTING_OPTION (CODE,NAME_AM,NAME_EN)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN)
GO
