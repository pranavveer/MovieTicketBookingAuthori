# JWT Authentication Implementation

## Security settings
The JWT signing key is stored with .NET User Secrets, not in appsettings.json.
Generate a new Base64 key and set it in the project directory:

```powershell
$bytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($bytes)
$key = [Convert]::ToBase64String($bytes)
$rng.Dispose()
dotnet user-secrets set "Jwt:Key" $key
```

Optional first Admin account (set all values as User Secrets; do not commit them):

```powershell
dotnet user-secrets set "SeedAdmin:Email" "admin@yourdomain.test"
dotnet user-secrets set "SeedAdmin:Username" "admin"
dotnet user-secrets set "SeedAdmin:Password" "Use-A-Unique-Long-Password!"
```

Public registration always creates the Customer role. It does not accept a role from the request.

## Apply the database changes
Open a terminal in the folder containing MovieTicketBooking.csproj:

```powershell
dotnet restore
dotnet build
dotnet ef migrations add AddUsersAndBookingOwnership
dotnet ef database update
```

Inspect the generated migration before applying it if your existing Bookings table already contains rows. The new UserId foreign key requires a valid owner for historical bookings; assign those rows to an appropriate existing user in a reviewed data migration rather than guessing.

## Run
```powershell
dotnet run
```

## Endpoints
- POST `/api/auth/register` — creates a Customer account.
- POST `/api/auth/login` — returns a JWT.
- GET `/api/movies`, `/api/movies/{id}` — public.
- POST/PUT/DELETE `/api/movies` — Admin only.
- GET `/api/bookings/mine` — Customer's bookings.
- POST `/api/bookings` — Customer creates a booking for their own account.
- GET `/api/bookings/{id}/mine` — Customer reads only their own booking.
- PUT `/api/bookings/{id}/cancel-mine` — Customer cancels only their own booking.
- GET `/api/bookings`, `/api/bookings/{id}`, `/api/bookings/show/{showId}` — Admin only.
- PUT `/api/bookings/{id}/cancel` — Admin cancellation.

All write operations for theatres, screens, seats and shows are Admin-only. Their read endpoints remain public.

## Postman
For login, send JSON:
```json
{
  "email": "admin@yourdomain.test",
  "password": "Use-A-Unique-Long-Password!"
}
```
Copy `token` from the response. For protected requests choose Authorization > Bearer Token and paste it.
