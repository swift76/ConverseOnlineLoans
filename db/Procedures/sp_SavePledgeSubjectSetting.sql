create or alter procedure sp_SavePledgeSubjectSetting(
	@SCORECARD_CODE		varchar(3)
	,@PLEDGE_SUBJECT	nvarchar(1000)
	,@SCORE				int
)
AS
	insert into PLEDGE_SUBJECT_SETTING(SCORECARD_CODE, PLEDGE_SUBJECT, SCORE)
	values (@SCORECARD_CODE, @PLEDGE_SUBJECT, @SCORE)
GO
