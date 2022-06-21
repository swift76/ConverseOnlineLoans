if exists (select * from sys.objects where name='sp_UpdateCustomerUserRegistration' and type='P')
	drop procedure dbo.sp_UpdateCustomerUserRegistration
GO

create procedure dbo.sp_UpdateCustomerUserRegistration(@PROCESS_ID			uniqueidentifier,
													   @VERIFICATION_CODE	char(4))
AS
	BEGIN TRANSACTION

	BEGIN TRY
		if exists (select top 1 PROCESS_ID
				   from dbo.CUSTOMER_USER_REGISTRATION
				   where PROCESS_ID = @PROCESS_ID)
			update dbo.CUSTOMER_USER_REGISTRATION set
				VERIFICATION_CODE = @VERIFICATION_CODE
			where PROCESS_ID = @PROCESS_ID

		execute dbo.sp_GetCustomerUserRegistration @PROCESS_ID

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
