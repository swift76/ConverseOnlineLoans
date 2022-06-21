if exists (select * from sys.objects where name='sp_UpdateCustomerUserPassword' and type='P')
	drop procedure dbo.sp_UpdateCustomerUserPassword
GO

create procedure dbo.sp_UpdateCustomerUserPassword(	@PROCESS_ID				uniqueidentifier,
													@MOBILE_PHONE			varchar(20),
													@VERIFICATION_CODE_HASH	varchar(1000),
													@PASSWORD_HASH			varchar(1000))
AS
	IF EXISTS (
		SELECT *
		FROM dbo.CUSTOMER_USER_PASSWORD_RESET
		WHERE PROCESS_ID = @PROCESS_ID
		AND HASH = @VERIFICATION_CODE_HASH
		AND LOGIN = @MOBILE_PHONE
		AND EXPIRES_ON > GETDATE())
		begin
			declare @SOCIAL_CARD_NUMBER char(10)

			select @SOCIAL_CARD_NUMBER = SOCIAL_CARD_NUMBER
			from dbo.CUSTOMER_USER as cu
			join dbo.APPLICATION_USER as au
				on cu.APPLICATION_USER_ID = au.ID
			where cu.MOBILE_PHONE = @MOBILE_PHONE

			UPDATE dbo.APPLICATION_USER SET HASH = @PASSWORD_HASH WHERE LOGIN = @SOCIAL_CARD_NUMBER
		end
	ELSE
	begin
		declare @ErrorMessage nvarchar(4000) = N'Մուտքագրված կոդը սխալ է'
		RAISERROR (@ErrorMessage, 17, 1)
	end
GO
