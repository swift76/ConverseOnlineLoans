if exists (select * from sys.objects where name='sp_SaveLoanType' and type='P')
	drop procedure dbo.sp_SaveLoanType
GO

create procedure dbo.sp_SaveLoanType(@CODE					char(2),
									 @NAME					nvarchar(50),
									 @NAME_EN				varchar(50),
									 @FROM_DATE				date,
									 @TO_DATE				date,
									 @REPAYMENT_DAY_FROM	tinyint,
									 @REPAYMENT_DAY_TO		tinyint,
									 @IS_OVERDRAFT			bit,
									 @IS_REPAY_DAY_FIXED	bit,
									 @IS_CARD_ACCOUNT		bit,
									 @IS_REPAY_START_DAY	bit,
									 @IS_REPAY_NEXT_MONTH	bit,
									 @REPAY_TRANSITION_DAY	tinyint,
									 @IS_PREAPPROVED		bit)
AS
	BEGIN TRANSACTION

	BEGIN TRY
		delete from dbo.LOAN_TYPE where CODE=@CODE
		insert into dbo.LOAN_TYPE (CODE,NAME_AM,NAME_EN,FROM_DATE,TO_DATE,REPAYMENT_DAY_FROM,REPAYMENT_DAY_TO,IS_OVERDRAFT,IS_REPAY_DAY_FIXED,IS_CARD_ACCOUNT,IS_REPAY_START_DAY,IS_REPAY_NEXT_MONTH,REPAY_TRANSITION_DAY,IS_PREAPPROVED)
			values (@CODE,dbo.ahf_ANSI2Unicode(@NAME),@NAME_EN,@FROM_DATE,@TO_DATE,@REPAYMENT_DAY_FROM,@REPAYMENT_DAY_TO,@IS_OVERDRAFT,@IS_REPAY_DAY_FIXED,@IS_CARD_ACCOUNT,@IS_REPAY_START_DAY,@IS_REPAY_NEXT_MONTH,@REPAY_TRANSITION_DAY,@IS_PREAPPROVED)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
GO
