# ASPCore.Northwind
ASP .NET Core application.

## Run API Tests
1. Open Epam.ASPCore.Northwind.sln and startup Epam.ASPCore.Northwind.WebUI project. (Open swagger - localhost:{port}/swagger)
2. Open Epam.ASPCore.Northwind.Tests.API.sln from ~\src\Epam.ASPCore.Northwind.Tests.API.
3. Now we can run API tests via Visual Studio test explorer or use something else. (for example resharper)

## Exists users and roles
| User  | Role | Password |
| ------------- | ------------- | ------------- |
| admin@admin.com  | Administrator  | P@ssword123  | 
| user@user.com  | dont have role  | P@ssword456  | 
| user1@user1.com  | dont have role  | P@ssword456  |

## User for test local reset password
| User  | Role | Password (LocalDB) | Password (email) |
| ------------- | ------------- | ------------- | ------------- |
| double.pavel.710@outlook.com  | dont have role  | P@ssword123  | P@velp@vel710  |

## Azure users info
If you want to login with Azure and have role "Administrator" you should change value in "AdminUserEmail" from appsettings.json to your email. User who logedin with Azure dont have a role by default.
