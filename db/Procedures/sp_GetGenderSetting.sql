create or alter procedure sp_GetGenderSetting(@GENDER bit)
AS
	select SCORE
	from GENDER_SETTING
	where GENDER=@GENDER
GO
