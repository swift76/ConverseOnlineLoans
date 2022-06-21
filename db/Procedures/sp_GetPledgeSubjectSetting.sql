create or alter procedure sp_GetPledgeSubjectSetting(@PLEDGE_SUBJECT	nvarchar(1000))
AS
	select SCORE
	from PLEDGE_SUBJECT_SETTING
	where PLEDGE_SUBJECT = @PLEDGE_SUBJECT
GO
