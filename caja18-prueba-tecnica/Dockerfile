#Start with the base .NET image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

#Set the working directory to /app
WORKDIR /app

#Copy the files from the host to the image and restore the dependencies
COPY *.csproj ./
RUN dotnet restore

#Copy the rest of the files and build the project
COPY . ./
RUN dotnet publish -c Release -o out

#Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env

#Set the working directory to /app
WORKDIR /app

#Copy the files from the build image to the runtime image
COPY --from=build-env /app/out .

#Set the entry point to the application
ENTRYPOINT ["dotnet", "caja18-prueba-tecnica.dll"]