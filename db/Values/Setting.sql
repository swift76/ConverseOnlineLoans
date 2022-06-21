insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('USER_PASSWORD_EXPIRY', '90', N'Օգտագործողի գաղտնաբառի ժամկետ /օր/')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('CREDIT_CARD_AUTH_TERM', '1800', N'Պլաստիկ քարտի նույնականացման կոդի վավերականության ժամկետ /վրկ/')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('CREDIT_CARD_TRY_COUNT', '5', N'Պլաստիկ քարտի վավերացման անհաջող փորձերի քանակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('CREDIT_CARD_SMS_COUNT', '3', N'Պլաստիկ քարտի վավերացման համար ուղարկվող հաղորդագրությունների քանակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('BANK_SERVER_DATABASE', '[(LOCAL)].bank.', N'Բանկային պահոց')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('SEND_SERVER_DATABASE', '', N'SMS/Email ուղարկելու պահոց')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('FILE_MAX_SIZE', '512000', N'Վերբեռնվող ֆայլի առավելագույն չափ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('NORQ_TRY_COUNT', '5', N'NORQ հարցումների փորձերի քանակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('ACRA_TRY_COUNT', '5', N'ACRA հարցումների փորձերի քանակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('NORQ_CHECK_BIRTH_DATE', '0', N'Ստուգել ծննդյան ամսաթիվը NORQ հարցումների ժամանակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('NORQ_WORK_MONTH', '3', N'NORQ-ով գործող աշխատանքի ժամկետ /ամիս/')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('ACRA_REPORT_TYPE', '01', N'ACRA հարցումների հաշվետվության տեսակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('NORQ_CACHE_DAY', '0', N'NORQ-ի հարցումների կրկին օգտագործման ժամկետ /օր/')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('ACRA_CACHE_DAY', '0', N'ACRA-ի հարցումների կրկին օգտագործման ժամկետ /օր/')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('CLIENT_ONLY', '1', N'Համակարգը միայն հաճախորդների համար է')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('MOBILE_PHONE_AUTH_TERM', '1800', N'Բջջային հեռախոսի նույնականացման կոդի վավերականության ժամկետ /վրկ/')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('MOBILE_PHONE_SMS_COUNT', '3', N'Բջջային հեռախոսի վավերացման համար ուղարկվող հաղորդագրությունների քանակ')
GO
insert into SETTING (CODE, VALUE, DESCRIPTION)
values ('TAKE_NORQ_PREVIOUS_YEAR', '1', N'Վերցնել NORQ-ի անցած տարվա տվյալները')
GO
