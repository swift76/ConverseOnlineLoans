if exists (select * from sys.objects where name='sp_GetSettings' and type='P')
	drop procedure dbo.sp_GetSettings
GO

create procedure dbo.sp_GetSettings(@CODE varchar(30) = null)
AS
	select CODE, VALUE, DESCRIPTION
	from dbo.SETTING
	where CODE = isnull(@CODE,CODE)
GO
