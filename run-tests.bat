@echo off
echo Setting up environment for Allure step reporting...

:: Create necessary directories
if not exist ".\TestResults" mkdir ".\TestResults"
if not exist ".\allure-results" mkdir ".\allure-results"

:: Set critical environment variables for Allure step reporting
set ALLURE_RESULTS_DIRECTORY=.\allure-results
set ALLURE_FEATURE_STEPS_ENABLED=true
set ALLURE_FEATURE_LIFECYCLE_ENABLED=true
set ALLURE_STEPS=true
set ALLURE_FEATURE_STEPS_DARKEN=true
set ALLURE_LIFECYCLE_ENABLED=true

:: Clean previous results
if exist ".\allure-results\*.*" del /Q ".\allure-results\*.*"

echo Running tests with Allure step reporting enabled...
dotnet test --logger "trx;LogFileName=TestResults.trx" --results-directory ./TestResults --settings PlaywrightFramework.Tests/NUnit.runsettings

echo Copying Allure results...
if exist ".\bin\Debug\net9.0\allure-results" xcopy ".\bin\Debug\net9.0\allure-results" ".\allure-results" /E /I /Y
if exist ".\TestResults\allure-results" xcopy ".\TestResults\allure-results" ".\allure-results" /E /I /Y
if exist ".\PlaywrightFramework.Tests\bin\Debug\net9.0\allure-results" xcopy ".\PlaywrightFramework.Tests\bin\Debug\net9.0\allure-results" ".\allure-results" /E /I /Y

echo Generating Allure report...
allure generate .\allure-results --clean -o .\allure-report

echo Done!
