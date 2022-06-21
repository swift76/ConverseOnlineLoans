if exists (select * from sys.objects where name='sp_SaveApplicationScoringResult' and type='P')
	drop procedure dbo.sp_SaveApplicationScoringResult
GO

create procedure dbo.sp_SaveApplicationScoringResult(@APPLICATION_ID	uniqueidentifier,
													 @SCORING_NUMBER	tinyint,
													 @AMOUNT			money,
													 @INTEREST			money)
AS
	insert into dbo.APPLICATION_SCORING_RESULT
		(APPLICATION_ID,SCORING_NUMBER,AMOUNT,INTEREST)
	values
		(@APPLICATION_ID,@SCORING_NUMBER,@AMOUNT,@INTEREST)
GO
