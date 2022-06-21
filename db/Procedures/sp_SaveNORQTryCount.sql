﻿if exists (select * from sys.objects where name='sp_SaveNORQTryCount' and type='P')
	drop procedure dbo.sp_SaveNORQTryCount
GO

create procedure dbo.sp_SaveNORQTryCount(@APPLICATION_ID		uniqueidentifier)
AS
	update dbo.APPLICATION
		set NORQ_TRY_COUNT=NORQ_TRY_COUNT+1,TO_BE_SYNCHRONIZED=1
		where ID=@APPLICATION_ID
GO
