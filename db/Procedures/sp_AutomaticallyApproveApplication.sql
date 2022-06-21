if exists (select * from sys.objects where name='sp_AutomaticallyApproveApplication' and type='P')
	drop procedure dbo.sp_AutomaticallyApproveApplication
GO

create procedure dbo.sp_AutomaticallyApproveApplication(@ID					uniqueidentifier,
														@ISN				int,
														@HAS_BANK_CARD		bit,
														@CLIENT_CODE		char(8),
														@IS_DATA_COMPLETE	bit,
														@CUSTOMER_STATUS_ID	tinyint)
AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @CURRENT_STATUS tinyint

		select @CURRENT_STATUS = STATUS_ID from dbo.APPLICATION with (updlock) where ID = @ID

		if @CURRENT_STATUS <> 3
			RAISERROR ('Հայտի կարգավիճակն արդեն փոփոխվել է', 17, 1)

		update dbo.APPLICATION set
			HAS_BANK_CARD=@HAS_BANK_CARD,
			STATUS_ID=5,
			TO_BE_SYNCHRONIZED=0,
			ISN=@ISN,
			CLIENT_CODE=@CLIENT_CODE,
			IS_DATA_COMPLETE=@IS_DATA_COMPLETE,
			CUSTOMER_STATUS_ID=@CUSTOMER_STATUS_ID
		where ID=@ID

		if rtrim(isnull(@CLIENT_CODE,''))<>''
		begin
			declare @CUSTOMER_USER_ID int
			select @CUSTOMER_USER_ID=CUSTOMER_USER_ID from dbo.APPLICATION where ID=@ID
			if not @CUSTOMER_USER_ID is null
				update dbo.CUSTOMER_USER set CLIENT_CODE=@CLIENT_CODE
				where APPLICATION_USER_ID=@CUSTOMER_USER_ID
		end

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
