﻿if exists (select * from sys.objects where name='sp_SaveNORQQueryResult' and type='P')
	drop procedure dbo.sp_SaveNORQQueryResult
GO

create procedure dbo.sp_SaveNORQQueryResult(@APPLICATION_ID						uniqueidentifier,
											@FIRST_NAME							nvarchar(40),
											@LAST_NAME							nvarchar(40),
											@PATRONYMIC_NAME					nvarchar(40),
											@BIRTH_DATE							date,
											@GENDER								bit,
											@DISTRICT							nvarchar(40),
											@COMMUNITY							nvarchar(40),
											@STREET								nvarchar(100),
											@BUILDING							nvarchar(40),
											@APARTMENT							nvarchar(40),
											@FEE								money,
											@NON_BIOMETRIC_PASSPORT_NUMBER		char(9),
											@NON_BIOMETRIC_PASSPORT_ISSUE_DATE	date,
											@NON_BIOMETRIC_PASSPORT_EXPIRY_DATE	date,
											@NON_BIOMETRIC_PASSPORT_ISSUED_BY	char(3),
											@BIOMETRIC_PASSPORT_NUMBER			char(9),
											@BIOMETRIC_PASSPORT_ISSUE_DATE		date,
											@BIOMETRIC_PASSPORT_EXPIRY_DATE		date,
											@BIOMETRIC_PASSPORT_ISSUED_BY		char(3),
											@ID_CARD_NUMBER						char(9),
											@ID_CARD_ISSUE_DATE					date,
											@ID_CARD_EXPIRY_DATE				date,
											@ID_CARD_ISSUED_BY					char(3),
											@SOCIAL_CARD_NUMBER					char(10),
											@RESPONSE_XML						nvarchar(max) = null)
AS
	BEGIN TRANSACTION
	BEGIN TRY
		declare @CURRENT_STATUS tinyint,@CUSTOMER_USER_ID int

		select @CURRENT_STATUS = STATUS_ID
			,@CUSTOMER_USER_ID = CUSTOMER_USER_ID
		from dbo.APPLICATION with (updlock)
		where ID = @APPLICATION_ID

		if @CURRENT_STATUS <> 1
			RAISERROR (N'Հայտի կարգավիճակն արդեն փոփոխվել է', 17, 1)

		insert into dbo.NORQ_QUERY_RESULT
			(APPLICATION_ID,FIRST_NAME,LAST_NAME,PATRONYMIC_NAME,BIRTH_DATE,GENDER,
			DISTRICT,COMMUNITY,STREET,BUILDING,APARTMENT,
			NON_BIOMETRIC_PASSPORT_NUMBER,NON_BIOMETRIC_PASSPORT_ISSUE_DATE,NON_BIOMETRIC_PASSPORT_EXPIRY_DATE,NON_BIOMETRIC_PASSPORT_ISSUED_BY,
			BIOMETRIC_PASSPORT_NUMBER,BIOMETRIC_PASSPORT_ISSUE_DATE,BIOMETRIC_PASSPORT_EXPIRY_DATE,BIOMETRIC_PASSPORT_ISSUED_BY,
			ID_CARD_NUMBER,ID_CARD_ISSUE_DATE,ID_CARD_EXPIRY_DATE,ID_CARD_ISSUED_BY,
			SOCIAL_CARD_NUMBER,FEE,RESPONSE_XML)
		values
			(@APPLICATION_ID,@FIRST_NAME,@LAST_NAME,@PATRONYMIC_NAME,@BIRTH_DATE,@GENDER,
			@DISTRICT,@COMMUNITY,@STREET,@BUILDING,@APARTMENT,
			@NON_BIOMETRIC_PASSPORT_NUMBER,@NON_BIOMETRIC_PASSPORT_ISSUE_DATE,@NON_BIOMETRIC_PASSPORT_EXPIRY_DATE,@NON_BIOMETRIC_PASSPORT_ISSUED_BY,
			@BIOMETRIC_PASSPORT_NUMBER,@BIOMETRIC_PASSPORT_ISSUE_DATE,@BIOMETRIC_PASSPORT_EXPIRY_DATE,@BIOMETRIC_PASSPORT_ISSUED_BY,
			@ID_CARD_NUMBER,@ID_CARD_ISSUE_DATE,@ID_CARD_EXPIRY_DATE,@ID_CARD_ISSUED_BY,
			@SOCIAL_CARD_NUMBER,@FEE,@RESPONSE_XML)

		if @CUSTOMER_USER_ID=0
			update APPLICATION
			set FIRST_NAME_AM=@FIRST_NAME
				,LAST_NAME_AM=@LAST_NAME
				,PATRONYMIC_NAME_AM=@PATRONYMIC_NAME
			where ID = @APPLICATION_ID

		execute dbo.sp_ChangeApplicationStatus @APPLICATION_ID,2
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		declare @ErrorMessage varchar(4000)
		set @ErrorMessage = ERROR_MESSAGE()
		RAISERROR (@ErrorMessage, 17, 1)
		RETURN
	END CATCH
	COMMIT TRANSACTION
GO
