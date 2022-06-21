create or alter procedure sp_DeletePledgeSubjectSetting(@SCORECARD_CODE	varchar(3))
AS
	delete from PLEDGE_SUBJECT_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
GO
