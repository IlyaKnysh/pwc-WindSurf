# Playwright Test Framework with Jenkins Pipeline

This project demonstrates a Playwright test automation framework with Jenkins pipeline integration.

## Features

- Page Object Model design pattern
- Fluent step definitions
- Screenshot capture on test failures
- Trace capture for debugging
- Jenkins pipeline integration

## Jenkins Setup

### Prerequisites

- Docker and Docker Compose installed
- Git installed

### Architecture

- **Jenkins Container**: Built from `jenkins/jenkins:lts` with .NET 6.0 SDK installed
- **Test Execution**: Uses `mcr.microsoft.com/playwright/dotnet:v1.52.0-noble` Docker image as a Jenkins agent
- **Allure Reporting**: Integrated for comprehensive test reporting with build configuration visibility

### Getting Started

1. Clone this repository to your local machine
2. Build and start Jenkins:

```bash
docker-compose -f ci/docker-compose.yml up -d
```

3. Access Jenkins at http://localhost:8080
4. Check that Allure Plugin and Allure Commandline are installed
5. Set up a new Pipeline job in Jenkins:
   - Select "Pipeline script from SCM"
   - Choose Git as the SCM
   - Enter your repository URL (e.g., https://gitlab.nixdev.co/QA/PWDemo.git)
   - Specify the branch to build (e.g., */main)
   - Set the Script Path to "ci/Jenkinsfile"

### Pipeline Configuration

The pipeline is configured with the following parameters:

- **BASE_URL**: The base URL for your tests (default: https://www.saucedemo.com)
- **USERNAME**: Username for authentication (default: standard_user)
- **PASSWORD**: Password for authentication (default: secret_sauce1)
- **BROWSER_TYPE**: Browser to use for testing (choices: chromium, firefox, webkit)

## Allure Reporting Integration

Allure reporting has been integrated to provide rich, detailed test reports with the following features:

- Test steps visualization
- Screenshots attached to test steps and failures
- Trace files for debugging failed tests
- Parameter logging for better test understanding
- Test categorization by features, stories, and severity
- Build configuration parameters displayed in the report (BASE_URL, USERNAME, BROWSER_TYPE)

## Running Tests with Allure Reporting

### Prerequisites

- .NET 9.0 SDK
- Allure command-line tool (for report generation)

### Install Allure Command-line Tool

#### Using Scoop (Windows)
```
scoop install allure
```

#### Using Chocolatey (Windows)
```
choco install allure-commandline
```

#### Using Homebrew (macOS)
```
brew install allure
```

### Running Tests

#### Option 1: Using the Batch File (Recommended)

The project includes a batch file that automatically sets up the environment variables required for Allure reporting, creates necessary directories, and runs the tests:

1. Run the tests using the batch file:
```
run-tests.bat
```

This script will:
- Create the necessary directories (TestResults and allure-results)
- Set required environment variables for Allure reporting
- Run the tests with appropriate settings
- Copy Allure results to the correct location
- Generate the Allure report

2. After the batch file completes, you can open the Allure report:
```
allure open allure-report
```

#### Option 2: Run via IDE

Open Test Explorer and choose needed tests to run.


## Allure Annotations

The framework uses the following Allure annotations:

### Test Level Annotations
- `[AllureSuite]` - Groups tests by suite
- `[AllureFeature]` - Groups tests by feature
- `[AllureStory]` - Describes the user story
- `[AllureSeverity]` - Sets test importance
- `[AllureIssue]` - Links to issue tracker
- `[AllureTms]` - Links to test management system

### Step Level Annotations
- `[AllureStep]` - Marks a method as a test step
- Parameters in step annotations can be templated with `{param1}`, `{param2}`, etc.

### Known Issues and Limitations

#### Nested AllureStep Attributes
- **Best Practice**: When creating step methods that call other step methods, consider using only one level of `[AllureStep]` annotation or implement a different pattern to avoid the recursive tracking issue.

## Example

```csharp
[Test]
[AllureFeature("Login")]
[AllureStory("User can login with valid credentials")]
[AllureSeverity(SeverityLevel.critical)]
public async Task SuccessfulLogin_WithValidCredentials()
{
    // Test implementation
}

[AllureStep("Login with username {username} and password {password}")]
public async Task LoginAsync(string username, string password)
{
    // Step implementation
}
```
# Should be solved:
1. Resource management (Users, Servers and devices distribution)
2. SR status reporting
3. Implement strict and dynamic resource selection for tests
4. Selectors versioning management - could be solved with branching strategy

### Pipeline Stages

The Jenkins pipeline includes the following stages:

1. **Build**: Builds the solution
2. **Test**: Runs the tests with the configured parameters

### Test Framework Features

The Playwright test framework includes the following features:

- Screenshot capture on test failures (configured in BaseTest.cs)
- Trace capture for debugging (configured in BaseTest.cs)
- Environment-specific configuration through environment variables

### Allure Reporting

The pipeline automatically configures Allure reporting with the correct environment variables and directory structure. Key features include:

- Automatic generation of Allure reports after test execution
- Display of build configuration parameters in the report (BASE_URL, USERNAME, BROWSER_TYPE)
- Proper collection of test results from the correct directory
- Integration with Jenkins Allure plugin for report viewing

After the pipeline runs, you can view the Allure report directly in Jenkins using the published HTML report.