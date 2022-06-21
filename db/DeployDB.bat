@echo off

set ConfigFile=config.ini
SET DatabaseName=CONVERSE_LOAN_SCORING
for /F "usebackq eol=;" %%s in ("%ConfigFile%") do set %%s

SET SQLEXEC=sqlcmd 

IF DEFINED DB_ServerName SET SQLEXEC=%SQLEXEC% -S %DB_ServerName%
IF DEFINED DB_Username SET SQLEXEC=%SQLEXEC% -U %DB_Username%
IF DEFINED DB_Password SET SQLEXEC=%SQLEXEC% -P %DB_Password%
IF DEFINED DB_Name SET SQLEXEC=%SQLEXEC% -v DatabaseName=%DB_Name%

ECHO %SQLEXEC%

IF DEFINED RecreateDB %SQLEXEC% -Q "DROP DATABASE %DB_Name%"
IF DEFINED RecreateDB %SQLEXEC% -i Database.sql

for %%G in (Types/*.sql) do %SQLEXEC% -d %DB_Name% -i "Types/%%G"
for %%G in (Tables/*.sql) do %SQLEXEC% -d %DB_Name% -i "Tables/%%G"
for %%G in (Functions/*.sql) do %SQLEXEC% -d %DB_Name% -i "Functions/%%G"
for %%G in (Procedures/*.sql) do %SQLEXEC% -d %DB_Name% -i "Procedures/%%G"
for %%G in (Values/*.sql) do %SQLEXEC% -d %DB_Name% -i "Values/%%G"

for %%G in (Dummy/Functions/*.sql) do %SQLEXEC% -d %DB_Name% -i "Dummy/Functions/%%G"
for %%G in (Dummy/Procedures/*.sql) do %SQLEXEC% -d %DB_Name% -i "Dummy/Procedures/%%G"
for %%G in (Dummy/Values/*.sql) do %SQLEXEC% -d %DB_Name% -i "Dummy/Values/%%G"