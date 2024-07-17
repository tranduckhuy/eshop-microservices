#!/bin/bash

CATALOG_PREFIX="tests/Services/Catalog/Catalog."
BASKET_PREFIX="tests/Services/Basket/Basket."
DISCOUNT_PREFIX="tests/Services/Discount/Discount."

REPORT_SUFFIX=".Test/coverage.opencover.xml"

CATALOG_REPORT_PATHS="${CATALOG_PREFIX}Application${REPORT_SUFFIX},${CATALOG_PREFIX}Infrastructure${REPORT_SUFFIX},${CATALOG_PREFIX}API${REPORT_SUFFIX}"
BASKET_REPORT_PATHS="${BASKET_PREFIX}Application${REPORT_SUFFIX},${BASKET_PREFIX}Infrastructure${REPORT_SUFFIX},${BASKET_PREFIX}API${REPORT_SUFFIX}"
DISCOUNT_REPORT_PATHS="${DISCOUNT_PREFIX}Application${REPORT_SUFFIX},${DISCOUNT_PREFIX}Infrastructure${REPORT_SUFFIX},${DISCOUNT_PREFIX}API${REPORT_SUFFIX}"

# Run SonarQube begin step
./.sonar/scanner/dotnet-sonarscanner begin \
  /k:"tranduckhuy_eshop-microservices" \
  /o:"tranduckhuy" \
  /d:sonar.token="${SONAR_TOKEN}" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.cs.opencover.reportsPaths="$CATALOG_REPORT_PATHS,$BASKET_REPORT_PATHS,$DISCOUNT_REPORT_PATHS" \
  /d:sonar.coverage.exclusions="**Infrastructure/Data/**/*,**/Program.cs,**/*Extension*.cs, **/Validators/**, **/Migrations/**" \
  /d:sonar.cpd.exclusions="**/Program.cs,**/BaseController.cs, **/Validators/**, **/Migrations/**"

dotnet build EShop.sln --configuration Debug
dotnet test ${CATALOG_PREFIX}Application.Test/Catalog.Application.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test ${CATALOG_PREFIX}Infrastructure.Test/Catalog.Infrastructure.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test ${CATALOG_PREFIX}API.Test/Catalog.API.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test ${BASKET_PREFIX}Application.Test/Basket.Application.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test ${BASKET_PREFIX}API.Test/Basket.API.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet test ${DISCOUNT_PREFIX}Application.Test/Discount.Application.Test.csproj --no-build /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

./.sonar/scanner/dotnet-sonarscanner end /d:sonar.token="${SONAR_TOKEN}"
