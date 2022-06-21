if exists (select * from sys.objects where name='sp_SetSettingValue' and type='P')
	drop procedure dbo.sp_SetSettingValue
GO

create procedure dbo.sp_SetSettingValue(@CODE	varchar(30),
										@VALUE	varchar(max))
AS
	update dbo.SETTING
	set VALUE = @VALUE
	where CODE = @CODE
GO
