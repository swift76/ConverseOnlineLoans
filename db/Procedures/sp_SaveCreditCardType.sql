if exists (select * from sys.objects where name='sp_SaveCreditCardType' and type='P')
	drop procedure dbo.sp_SaveCreditCardType
GO

create procedure dbo.sp_SaveCreditCardType(@CODE				char(3),
										  @NAME_AM			nvarchar(50),
										  @NAME_EN			varchar(50),
										  @LOAN_TYPE_ID		char(2),
										  @CURRENCY_CODE	char(3))
AS
	insert into dbo.CREDIT_CARD_TYPE (CODE,NAME_AM,NAME_EN,LOAN_TYPE_ID,CURRENCY_CODE)
		values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN,@LOAN_TYPE_ID,@CURRENCY_CODE)
GO
