# 1. გაშვების გარემო (Runtime Stage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Render-ისთვის ვუთითებთ, რომ აპლიკაციამ მხოლოდ 8080 პორტზე იფუნქციონიროს HTTP-ით
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 2. აწყობის გარემო (Build Stage)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ვაკოპირებთ .csproj ფაილს მიმდინარე დირექტორიიდან და ვაკეთებთ Restore-ს
COPY ["BackpackStoreFS.csproj", "./"]
RUN dotnet restore "./BackpackStoreFS.csproj"

# ვაკოპირებთ აბსოლუტურად ყველა დანარჩენ ფაილს (Controllers, Services და ა.შ.)
COPY . .

# ვაშენებთ პროექტს Release რეჟიმში
RUN dotnet build "BackpackStoreFS.csproj" -c Release -o /app/build

# 3. გამოქვეყნების გარემო (Publish Stage)
FROM build AS publish
RUN dotnet publish "BackpackStoreFS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. საბოლოო იმიჯის შექმნა (Final Runtime Image)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackpackStoreFS.dll"]