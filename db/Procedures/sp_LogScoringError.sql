if exists (select * from sys.objects where name='sp_LogScoringError' and type='P')
	drop procedure dbo.sp_LogScoringError
GO

create procedure dbo.sp_LogScoringError(@APPLICATION_ID	uniqueidentifier = NULL,
										   @OPERATION		varchar(200),
										   @ERROR_MESSAGE	nvarchar(max))
AS
	insert into dbo.SCORING_ERROR
		(APPLICATION_ID,OPERATION,ERROR_MESSAGE)
	values
		(@APPLICATION_ID,@OPERATION,@ERROR_MESSAGE)
GO
