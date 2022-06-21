if exists (select * from sys.objects where name='sp_SaveLoanServiceCondition' and type='P')
	drop procedure dbo.sp_SaveLoanServiceCondition
GO

create procedure dbo.sp_SaveLoanServiceCondition(@LOAN_TYPE_CODE		char(2),
												 @SERVICE_TYPE_CODE		char(1),
												 @CURRENCY				char(3),
												 @CREDIT_CARD_TYPE_CODE	char(3),
												 @AMOUNT				money,
												 @INTEREST				money)
AS
	insert into dbo.LOAN_SERVICE_CONDITION (LOAN_TYPE_CODE,SERVICE_TYPE_CODE,CURRENCY,CREDIT_CARD_TYPE_CODE,AMOUNT,INTEREST)
		values (@LOAN_TYPE_CODE,@SERVICE_TYPE_CODE,@CURRENCY,@CREDIT_CARD_TYPE_CODE,@AMOUNT,@INTEREST)
GO
