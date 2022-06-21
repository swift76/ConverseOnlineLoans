create or alter procedure sp_GetScoringFicoValue(
	@FACTOR_ID 	tinyint,
	@VALUE		money
)
AS
	select top 1 SCORE
	from SCORING_FICO_VALUE
	where FACTOR_ID=@FACTOR_ID
		and @VALUE between FROM_VALUE and TO_VALUE
	order by ID
GO
