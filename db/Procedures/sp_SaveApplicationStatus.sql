if exists (select * from sys.objects where name='sp_SaveApplicationStatus' and type='P')
	drop procedure dbo.sp_SaveApplicationStatus
GO

create procedure dbo.sp_SaveApplicationStatus(@CODE			char(2),
											  @NAME_AM		nvarchar(50),
											  @NAME_EN		varchar(50),
											  @UI_NAME_AM	nvarchar(50) = null,
											  @UI_NAME_EN	varchar(50) = null)
AS
	BEGIN TRANSACTION

	BEGIN TRY
		delete from dbo.APPLICATION_STATUS where CODE=@CODE
		insert into dbo.APPLICATION_STATUS (CODE,NAME_AM,NAME_EN,UI_NAME_AM,UI_NAME_EN)
			values (@CODE,dbo.ahf_ANSI2Unicode(@NAME_AM),@NAME_EN,dbo.ahf_ANSI2Unicode(@UI_NAME_AM),@UI_NAME_EN)

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage nvarchar(4000) = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
GO
