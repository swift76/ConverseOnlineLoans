create or alter procedure sp_SavePledgeSubjectSetting(
	@PLEDGE_SUBJECT	nvarchar(1000)
	,@SCORE			int
)
AS
	insert into PLEDGE_SUBJECT_SETTING(PLEDGE_SUBJECT, SCORE)
	values (@PLEDGE_SUBJECT, @SCORE)
GO
