## App Name
Derek Notifications

## Project Name
DerekNotifications.sln

## Description
Used to post issues in YouTrack and send emails

## Additional Notes
- .NET project
- Runs in a Docker container
- Update ruff.com register scripts to forward to this service
  - /home/register/public_html/regemail_derek.php
  - /home/register/public_html/regemail_datastore.php
  - /home/register/public_html/efile_memo_understand.php
  - /home/register/public_html/efile_email16.php

## Get Source
```aiignore
git clone https://github.com/rsi77/DerekNotifications.git
```

## How to run locally
```aiignore
cd <root-folder>
dotnet run --project DerekNotifications/DerekNotifications.csproj
```

## How to Build
```aiignore
cd <root-folder>
dotnet build
```

## How to Clean
```aiignore
cd <root-folder>
dotnet clean
```

## How to Test
```aiignore
cd <root-folder>
dotnet test
```

## Depends On
#### None
