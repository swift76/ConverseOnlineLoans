if exists (select * from sys.objects where name='sp_EmptyDirectoriesBeforeSynchronization' and type='P')
	drop procedure dbo.sp_EmptyDirectoriesBeforeSynchronization
GO

create procedure dbo.sp_EmptyDirectoriesBeforeSynchronization
AS
	BEGIN TRANSACTION

	BEGIN TRY
		truncate table dbo.DOCUMENT_TYPE
		truncate table dbo.COMMUNICATION_TYPE
		truncate table dbo.LOAN_GETTING_OPTION
		truncate table dbo.ORGANIZATION_ACTIVITY
		truncate table dbo.CARD_RECEIVING_OPTIONS
		truncate table dbo.COUNTRY
		truncate table dbo.STATE
		truncate table dbo.CITY
		truncate table dbo.APPLICATION_SCAN_TYPE
		truncate table dbo.MONTHLY_NET_SALARY

		truncate table dbo.WORKING_EXPERIENCE
		truncate table dbo.FAMILY_STATUS
		truncate table dbo.BANK_BRANCH

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
GO
