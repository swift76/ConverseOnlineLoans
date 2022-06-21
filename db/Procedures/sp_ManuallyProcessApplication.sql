if exists (select * from sys.objects where name='sp_ManuallyProcessApplication' and type='P')
	drop procedure dbo.sp_ManuallyProcessApplication
GO

create procedure dbo.sp_ManuallyProcessApplication(@ID				uniqueidentifier,
												   @ISN				int,
												   @MANUAL_REASON	varchar(100),
												   @CLIENT_CODE		char(8))
AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @CURRENT_STATUS tinyint

		select @CURRENT_STATUS = STATUS_ID from dbo.APPLICATION with (updlock) where ID = @ID

		if @CURRENT_STATUS <> 3
			RAISERROR ('Հայտի կարգավիճակն արդեն փոփոխվել է', 17, 1)

		update dbo.APPLICATION set
			MANUAL_REASON=dbo.ahf_ANSI2Unicode(@MANUAL_REASON),
			STATUS_ID=7,
			TO_BE_SYNCHRONIZED=0,
			ISN=@ISN,
			CLIENT_CODE=@CLIENT_CODE
		where ID=@ID

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
