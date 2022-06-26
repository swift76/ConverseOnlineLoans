create function dbo.f_CalculateFicoScore(@APPLICATION_ID	uniqueidentifier)
RETURNS int
AS
BEGIN
	declare @PAYMENT_HISTORY_YEAR tinyint
		,@HAD_LOAN_YEAR tinyint
		,@NEW_LOAN_MONTH tinyint
		,@NEW_QUERY_MONTH tinyint
		,@NEW_RATIO_MONTH tinyint
		,@NON_BANK_INTEREST money
	select @PAYMENT_HISTORY_YEAR=PAYMENT_HISTORY_YEAR
		,@HAD_LOAN_YEAR=HAD_LOAN_YEAR
		,@NEW_LOAN_MONTH=NEW_LOAN_MONTH
		,@NEW_QUERY_MONTH=NEW_QUERY_MONTH
		,@NEW_RATIO_MONTH=NEW_RATIO_MONTH
		,@NON_BANK_INTEREST=NON_BANK_INTEREST
	from FICO_SETTING_PARAMETER with (nolock)

	declare @Factor1Score money,@Factor2Score money,@Factor3Score money,@Factor4Score money,@Factor5Score money,@Factor6Score money,@Factor7Score money,@Factor8Score money,@Factor9Score money

	declare @SumOverdueDays int,@MaxOverdueDays int

	if @PAYMENT_HISTORY_YEAR=1
	begin
		select @SumOverdueDays=SUM_OVERDUE_DAYS_Y1_Y1
			,@MaxOverdueDays=MAX_OVERDUE_DAYS_Y1_Y1
		from ACRA_QUERY_RESULT_DETAILS with (nolock)
		where APPLICATION_ID=@APPLICATION_ID
			and IS_GUARANTEE=0
	end
	else if @PAYMENT_HISTORY_YEAR=2
	begin
		select @SumOverdueDays=SUM_OVERDUE_DAYS_Y1_Y2
			,@MaxOverdueDays=MAX_OVERDUE_DAYS_Y1_Y2
		from ACRA_QUERY_RESULT_DETAILS with (nolock)
		where APPLICATION_ID=@APPLICATION_ID
			and IS_GUARANTEE=0
	end
	else if @PAYMENT_HISTORY_YEAR=3
	begin
		select @SumOverdueDays=SUM_OVERDUE_DAYS_Y1_Y3
			,@MaxOverdueDays=MAX_OVERDUE_DAYS_Y1_Y3
		from ACRA_QUERY_RESULT_DETAILS with (nolock)
		where APPLICATION_ID=@APPLICATION_ID
			and IS_GUARANTEE=0
	end
	else if @PAYMENT_HISTORY_YEAR=4
	begin
		select @SumOverdueDays=SUM_OVERDUE_DAYS_Y1_Y4
			,@MaxOverdueDays=MAX_OVERDUE_DAYS_Y1_Y4
		from ACRA_QUERY_RESULT_DETAILS with (nolock)
		where APPLICATION_ID=@APPLICATION_ID
			and IS_GUARANTEE=0
	end
	else if @PAYMENT_HISTORY_YEAR=5
	begin
		select @SumOverdueDays=SUM_OVERDUE_DAYS_Y1_Y5
			,@MaxOverdueDays=MAX_OVERDUE_DAYS_Y1_Y5
		from ACRA_QUERY_RESULT_DETAILS with (nolock)
		where APPLICATION_ID=@APPLICATION_ID
			and IS_GUARANTEE=0
	end

	select top 1 @Factor1Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=1
		and isnull(@SumOverdueDays,0) between FROM_VALUE and TO_VALUE
	order by ID

	select top 1 @Factor2Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=2
		and isnull(@MaxOverdueDays,0) between FROM_VALUE and TO_VALUE
	order by ID

	declare @TotalContractAmount money,@TotalDebt money,@TotalLoanCount money,@DebtContractRatio money,@TotalNonBankCount money,@NonBankLoanRatio money
	select @TotalContractAmount=sum(CONTRACT_AMOUNT)
		,@TotalDebt=sum(DEBT)
		,@TotalLoanCount=count(STATUS)
	from ACRA_QUERY_RESULT_DETAILS with (nolock)
	where APPLICATION_ID=@APPLICATION_ID
		and IS_GUARANTEE=0
		and upper(trim(STATUS))=N'ԳՈՐԾՈՂ'

	if @TotalContractAmount>0
		set @DebtContractRatio=100*@TotalDebt/@TotalContractAmount
	else
		set @DebtContractRatio=0

	select top 1 @Factor3Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=3
		and isnull(@DebtContractRatio,0) between FROM_VALUE and TO_VALUE
	order by ID

	select top 1 @Factor4Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=4
		and isnull(@TotalLoanCount,0) between FROM_VALUE and TO_VALUE
	order by ID

	select @TotalNonBankCount=count(STATUS)
	from ACRA_QUERY_RESULT_DETAILS with (nolock)
	where APPLICATION_ID=@APPLICATION_ID
		and IS_GUARANTEE=0
		and upper(trim(STATUS))=N'ԳՈՐԾՈՂ'
		and INTEREST_RATE>@NON_BANK_INTEREST

	if @TotalLoanCount>0
		set @NonBankLoanRatio=100*@TotalNonBankCount/@TotalLoanCount
	else
		set @NonBankLoanRatio=0

	select top 1 @Factor5Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=5
		and isnull(@NonBankLoanRatio,0) between FROM_VALUE and TO_VALUE
	order by ID

	declare @ToDate date=getdate()
	declare @FromDate date=DATEADD(day,1,DATEADD(year,-@HAD_LOAN_YEAR,@ToDate))
	declare @HadLoanDayCount int;

	with AllDates As
	(
		select DATEADD(day,1,@FromDate) as Date
		UNION ALL
		select DATEADD(day,1,Date) from AllDates
		where Date <= @ToDate
	)

	select @HadLoanDayCount=count(distinct d.Date)
	from AllDates d
	join ACRA_QUERY_RESULT_DETAILS a with (nolock)
		on d.Date between a.FROM_DATE and a.TO_DATE
	where a.APPLICATION_ID=@APPLICATION_ID
		and a.IS_GUARANTEE=0
	option (maxrecursion 0)

	select top 1 @Factor6Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=6
		and isnull(@HadLoanDayCount,0) between FROM_VALUE and TO_VALUE
	order by ID

	declare @NewLoanCount int
	set @FromDate=DATEADD(day,1,DATEADD(month,-@NEW_LOAN_MONTH,@ToDate))

	select @NewLoanCount=count(STATUS)
	from ACRA_QUERY_RESULT_DETAILS with (nolock)
	where APPLICATION_ID=@APPLICATION_ID
		and IS_GUARANTEE=0
		and FROM_DATE>=@FromDate

	select top 1 @Factor7Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=7
		and isnull(@NewLoanCount,0) between FROM_VALUE and TO_VALUE
	order by ID

	declare @NewQueryCount int
	set @FromDate=DATEADD(day,1,DATEADD(month,-@NEW_QUERY_MONTH,@ToDate))

	select @NewQueryCount=count(*)
	from ACRA_QUERY_RESULT_QUERIES with (nolock)
	where APPLICATION_ID=@APPLICATION_ID
		and upper(trim(REASON))=N'ՆՈՐ ՎԱՐԿԱՅԻՆ ԴԻՄՈՒՄ'
		and DATE>=@FromDate
	group by DATE,BANK_NAME

	select top 1 @Factor8Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=8
		and isnull(@NewQueryCount,0) between FROM_VALUE and TO_VALUE
	order by ID

	declare @QueryLoanRatio money
	set @FromDate=DATEADD(day,1,DATEADD(month,-@NEW_RATIO_MONTH,@ToDate))

	select @NewLoanCount=count(STATUS)
	from ACRA_QUERY_RESULT_DETAILS with (nolock)
	where APPLICATION_ID=@APPLICATION_ID
		and IS_GUARANTEE=0
		and FROM_DATE>=@FromDate

	select @NewQueryCount=count(*)
	from ACRA_QUERY_RESULT_QUERIES with (nolock)
	where APPLICATION_ID=@APPLICATION_ID
		and upper(trim(REASON))=N'ՆՈՐ ՎԱՐԿԱՅԻՆ ԴԻՄՈՒՄ'
		and DATE>=@FromDate
	group by DATE,BANK_NAME

	if @NewQueryCount>0
		set @QueryLoanRatio=100*@NewLoanCount/@NewQueryCount
	else
		set @QueryLoanRatio=0

	select top 1 @Factor9Score=SCORE
	from SCORING_FICO_VALUE with (nolock)
	where FACTOR_ID=9
		and isnull(@QueryLoanRatio,0) between FROM_VALUE and TO_VALUE
	order by ID

	declare @Result int=convert(int,isnull(@Factor1Score,0)+
		isnull(@Factor2Score,0)+
		isnull(@Factor3Score,0)+
		isnull(@Factor4Score,0)+
		isnull(@Factor5Score,0)+
		isnull(@Factor6Score,0)+
		isnull(@Factor7Score,0)+
		isnull(@Factor8Score,0)+
		isnull(@Factor9Score,0))

	return @Result
END
GO
