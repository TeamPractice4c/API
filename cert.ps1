New-Item -Path "$env:USERPROFILE\.aspnet\https" -ItemType Directory -Force
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"  -p 1245780r
dotnet dev-certs https --trust