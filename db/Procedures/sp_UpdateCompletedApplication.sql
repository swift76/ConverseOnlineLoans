if exists (select * from sys.objects where name='sp_SaveCompletedApplication' and type='P')
	drop procedure dbo.sp_SaveCompletedApplication
GO

create procedure dbo.sp_SaveCompletedApplication(@APPLICATION_ID 				uniqueidentifier,
											     @EFFECTIVE_INTEREST			money = null,
											     @MONTHLY_PAID_AMOUNT			money = null,
											     @FIRST_PAID_INTEREST			money = null,
											     @FIRST_PAID_PRINCIPAL_AMOUNT	money = null,
											     @TOTAL_PAID_AMOUNT				money = null,
											     @TOTAL_PAID_INTEREST			money = null)
AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @STATUS_ID tinyint

		select @STATUS_ID = STATUS_ID
		from dbo.APPLICATION with (updlock) where ID = @APPLICATION_ID

		--if not @STATUS_ID in (5, 8) -- TODO Correct the status ID's
		--	RAISERROR ('????? ????????? ??????? ????????????? ??', 17, 1)

		update dbo.APPLICATION set
			EFFECTIVE_INTEREST			= isnull(@EFFECTIVE_INTEREST,			EFFECTIVE_INTEREST),
			MONTHLY_PAID_AMOUNT			= isnull(@MONTHLY_PAID_AMOUNT,			MONTHLY_PAID_AMOUNT),
			FIRST_PAID_INTEREST			= isnull(@FIRST_PAID_INTEREST,			FIRST_PAID_INTEREST),
			FIRST_PAID_PRINCIPAL_AMOUNT	= isnull(@FIRST_PAID_PRINCIPAL_AMOUNT,	FIRST_PAID_PRINCIPAL_AMOUNT),
			TOTAL_PAID_AMOUNT			= isnull(@TOTAL_PAID_AMOUNT,			TOTAL_PAID_AMOUNT),
			TOTAL_PAID_INTEREST			= isnull(@TOTAL_PAID_INTEREST,			TOTAL_PAID_INTEREST)
		where ID = @APPLICATION_ID

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
GO
