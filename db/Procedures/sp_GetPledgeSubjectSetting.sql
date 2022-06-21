create or alter procedure sp_GetPledgeSubjectSetting(
	@SCORECARD_CODE		varchar(3)
	,@PLEDGE_SUBJECT	nvarchar(1000)
)
AS
	select SCORE
	from PLEDGE_SUBJECT_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
		and PLEDGE_SUBJECT = @PLEDGE_SUBJECT
GO
