FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY ./Epam.ASPCore.Northwind/*.csproj ./Epam.ASPCore.Northwind/
COPY ./Epam.ASPCore.Northwind.Tests/*.csproj ./Epam.ASPCore.Northwind.Tests/
COPY ./Epam.ASPCore.Northwind.Domain/*.csproj ./Epam.ASPCore.Northwind.Domain/
COPY ./Microsoft.Identity.Web/*.csproj ./Microsoft.Identity.Web/
RUN dotnet restore

# Copy everything else and build
COPY ./Epam.ASPCore.Northwind/. ./Epam.ASPCore.Northwind/
COPY ./Epam.ASPCore.Northwind.Tests/. ./Epam.ASPCore.Northwind.Tests/
COPY ./Epam.ASPCore.Northwind.Domain/. ./Epam.ASPCore.Northwind.Domain/
COPY ./Microsoft.Identity.Web/. ./Microsoft.Identity.Web/
WORKDIR /app/Epam.ASPCore.Northwind
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/Epam.ASPCore.Northwind/out ./
ENTRYPOINT ["dotnet", "Epam.ASPCore.Northwind.WebUI.dll"]