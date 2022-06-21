if exists (select * from sys.objects where name='sp_GetDocumentTypes' and type='P')
	drop procedure dbo.sp_GetDocumentTypes
GO

create procedure dbo.sp_GetDocumentTypes(@LANGUAGE_CODE	char(2))
AS
	select CODE,
		case upper(@LANGUAGE_CODE)
			when 'AM' then NAME_AM
			else NAME_EN
		end as NAME
	from dbo.DOCUMENT_TYPE
GO
