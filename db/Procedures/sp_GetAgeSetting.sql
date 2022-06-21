create or alter procedure sp_GetAgeSetting(@AGE tinyint)
AS
	select SCORE
	from AGE_SETTING
	where AGE = @AGE
GO
