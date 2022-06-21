if exists (select * from sys.objects where name='sp_DeleteApplication' and type='P')
	drop procedure dbo.sp_DeleteApplication
GO

create procedure dbo.sp_DeleteApplication(@ID					uniqueidentifier,
										  @OPERATION_DETAILS	nvarchar(max))

AS
	BEGIN TRANSACTION

	BEGIN TRY
		declare @STATUS_ID tinyint, @CUSTOMER_USER_ID int, @SHOP_USER_ID int, @PARTNER_COMPANY_CODE varchar(8), @LOAN_TYPE_ID char(2)

		select @STATUS_ID = STATUS_ID, @CUSTOMER_USER_ID = CUSTOMER_USER_ID, @SHOP_USER_ID = SHOP_USER_ID, @PARTNER_COMPANY_CODE = PARTNER_COMPANY_CODE, @LOAN_TYPE_ID = LOAN_TYPE_ID
		from dbo.APPLICATION with (updlock)
		where ID = @ID

		if @STATUS_ID <> 0
			RAISERROR ('Հայտը նախնական կարգավիճակում չէ', 17, 1)

		insert into dbo.APPLICATION_OPERATION_LOG
				(CUSTOMER_USER_ID, SHOP_USER_ID, PARTNER_COMPANY_CODE, LOAN_TYPE_ID, OPERATION_CODE, APPLICATION_ID, OPERATION_DETAILS)
		values
				(@CUSTOMER_USER_ID, @SHOP_USER_ID, @PARTNER_COMPANY_CODE, @LOAN_TYPE_ID, 'DELETE', @ID, @OPERATION_DETAILS)

		delete from dbo.APPLICATION_SCAN where APPLICATION_ID = @ID
		delete from dbo.APPLICATION where ID = @ID

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
	END CATCH
GO
