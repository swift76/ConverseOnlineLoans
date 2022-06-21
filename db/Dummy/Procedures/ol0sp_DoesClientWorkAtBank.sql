if exists (select * from sys.objects where name='ol0sp_DoesClientWorkAtBank' and type='P')
	drop procedure dbo.ol0sp_DoesClientWorkAtBank
GO

create procedure ol0sp_DoesClientWorkAtBank(@SocialCardCode	nvarchar(10))
AS

SELECT 0

GO