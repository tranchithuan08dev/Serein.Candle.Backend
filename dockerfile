# ======================================================================
# GIAI ĐOẠN 1: BUILD (Sử dụng .NET SDK để biên dịch mã nguồn)
# ======================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG CONFIGURATION=Release
WORKDIR /src

# 1. Sao chép file Solution và các Project file (.csproj) để Restore Dependencies
# Cần copy các file .csproj của tất cả các project trong solution để đảm bảo restore thành công.
# Giả định file solution là Serein.Candle.Backend.sln
COPY Serein.Candle.Backend.sln . 
COPY Serein.Candle.WebApi/Serein.Candle.WebApi.csproj Serein.Candle.WebApi/
COPY Serein.Candle.Application/Serein.Candle.Application.csproj Serein.Candle.Application/
COPY Serein.Candle.Domain/Serein.Candle.Domain.csproj Serein.Candle.Domain/
COPY Serein.Candle.Infrastructure/Serein.Candle.Infrastructure.csproj Serein.Candle.Infrastructure/

# 2. Chạy Restore Dependencies (Các thư viện NuGet)
RUN dotnet restore

# 3. Sao chép toàn bộ mã nguồn còn lại
COPY . .

# 4. Build và Publish (Chỉ Publish project WebApi - Điểm khởi động)
WORKDIR /src/Serein.Candle.WebApi
RUN dotnet publish "Serein.Candle.WebApi.csproj" -c $CONFIGURATION -o /app/publish

# ======================================================================
# GIAI ĐOẠN 2: FINAL (Chỉ chứa Runtime - Hình ảnh nhẹ và bảo mật hơn)
# ======================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 1. Sao chép các file đã publish từ giai đoạn build
COPY --from=build /app/publish .

# 2. Cấu hình Cổng (Port)
# Render thường sử dụng cổng 8080 cho các dịch vụ Docker/Web Service
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 3. Điểm khởi động (Entrypoint)
# Chạy file DLL của project WebApi
ENTRYPOINT ["dotnet", "Serein.Candle.WebApi.dll"]