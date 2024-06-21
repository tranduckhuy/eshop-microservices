#!/bin/bash

REPORT_PATHS="tests/Services/Catalog/Catalog.Application.Test/coverage.opencover.xml,tests/Services/Catalog/Catalog.Infrastructure.Test/coverage.opencover.xml,tests/Services/Catalog/Catalog.API.Test/coverage.opencover.xml"

# Run SonarQube begin step
./.sonar/scanner/dotnet-sonarscanner begin \
  /k:"tranduckhuy_eshop-microservices" \
  /o:"tranduckhuy" \
  /d:sonar.token="${SONAR_TOKEN}" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.cs.opencover.reportsPaths="$REPORT_PATHS" \
  /d:sonar.coverage.exclusions="src/Services/Catalog/Catalog.Infrastructure/Data/**/*,**/Program.cs,**/Extensions.cs"

dotnet build EShop.sln --configuration Debug
dotnet test tests/Services/Catalog/Catalog.Application.Test/Catalog.Application.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test tests/Services/Catalog/Catalog.Infrastructure.Test/Catalog.Infrastructure.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test tests/Services/Catalog/Catalog.API.Test/Catalog.API.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

./.sonar/scanner/dotnet-sonarscanner end /d:sonar.token="${SONAR_TOKEN}"
