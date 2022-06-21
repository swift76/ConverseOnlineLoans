if exists (select * from sys.objects where name='sp_GetCachedNORQResponse' and type='P')
	drop procedure dbo.sp_GetCachedNORQResponse
GO

create procedure dbo.sp_GetCachedNORQResponse(@SOCIAL_CARD_NUMBER	char(10))
AS
	declare @DayCount int
	select @DayCount=convert(int,VALUE) from SETTING where CODE='NORQ_CACHE_DAY'

	declare @DateTo date=getdate()
	declare @DateFrom date=dateadd(month,-@DayCount,@DateTo)

	select top 1 n.RESPONSE_XML
	from APPLICATION a
	join NORQ_QUERY_RESULT n
		on a.ID=n.APPLICATION_ID
	where a.SOCIAL_CARD_NUMBER=@SOCIAL_CARD_NUMBER
		and convert(date,n.QUERY_DATE) between @DateFrom and @DateTo
	order by n.QUERY_DATE desc
GO
