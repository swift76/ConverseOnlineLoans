if exists (select * from sys.objects where name='sp_SaveLoanLimit' and type='P')
	drop procedure dbo.sp_SaveLoanLimit
GO

create procedure dbo.sp_SaveLoanLimit(@LOAN_TYPE_CODE	char(2),
										 @CURRENCY			char(3),
										 @NAME				nvarchar(50),
										 @NAME_EN			varchar(50),
										 @FROM_AMOUNT		money,
										 @TO_AMOUNT			money)
AS
	insert into dbo.LOAN_LIMIT (LOAN_TYPE_CODE,CURRENCY,NAME_AM,NAME_EN,FROM_AMOUNT,TO_AMOUNT)
		values (@LOAN_TYPE_CODE,@CURRENCY,dbo.ahf_ANSI2Unicode(@NAME),@NAME_EN,@FROM_AMOUNT,@TO_AMOUNT)
GO
