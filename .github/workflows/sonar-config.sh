#!/bin/bash

CATALOG_PREFIX="tests/Services/Catalog/Catalog."
BASKET_PREFIX="tests/Services/Basket/Basket."

REPORT_SUFFIX=".Test/coverage.opencover.xml"

CATALOG_REPORT_PATHS="${CATALOG_PREFIX}Application${REPORT_SUFFIX},${CATALOG_PREFIX}Infrastructure${REPORT_SUFFIX},${CATALOG_PREFIX}API${REPORT_SUFFIX}"
BASKET_REPORT_PATHS="${BASKET_PREFIX}Application${REPORT_SUFFIX},${BASKET_PREFIX}Infrastructure${REPORT_SUFFIX},${BASKET_PREFIX}API${REPORT_SUFFIX}"

# Run SonarQube begin step
./.sonar/scanner/dotnet-sonarscanner begin \
  /k:"tranduckhuy_eshop-microservices" \
  /o:"tranduckhuy" \
  /d:sonar.token="${SONAR_TOKEN}" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.cs.opencover.reportsPaths="$CATALOG_REPORT_PATHS" \
  /d:sonar.coverage.exclusions="src/Services/Catalog/Catalog.Infrastructure/Data/**/*,**/Program.cs,**/Extensions.cs" \
  /d:sonar.cpd.exclusions="**/Program.cs,**/BaseController.cs"

dotnet build EShop.sln --configuration Debug
dotnet test tests/Services/Catalog/Catalog.Application.Test/Catalog.Application.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test tests/Services/Catalog/Catalog.Infrastructure.Test/Catalog.Infrastructure.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test tests/Services/Catalog/Catalog.API.Test/Catalog.API.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

./.sonar/scanner/dotnet-sonarscanner end /d:sonar.token="${SONAR_TOKEN}"
