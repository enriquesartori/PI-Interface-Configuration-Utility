# Use the official .NET SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . ./

# Build for Windows x64
RUN dotnet publish --configuration Release --runtime win-x64 --self-contained true --output out/win-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true

# Build for Windows x86  
RUN dotnet publish --configuration Release --runtime win-x86 --self-contained true --output out/win-x86 -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true

# Use a minimal base image for the final stage
FROM scratch AS export-stage
COPY --from=build-env /app/out/ / 