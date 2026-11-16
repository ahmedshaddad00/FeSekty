# FeSekty Mobile API – Developer Guide (Register, Login, Testing)

Audience: Flutter (Android + iOS) developers. Status: development (localhost). All examples were tested against the running backend.

## 1. Environments

| Target | Base URL |
|---|---|
| Android emulator | `http://10.0.2.2:5000` |
| iOS simulator | `http://localhost:5000` |
| Swagger (API list) | `http://localhost:5000/swagger` (open on the dev PC) |

Backend must be started on the dev PC in Development mode:
```
cd src/FeSekty.Web
ASPNETCORE_ENVIRONMENT=Development dotnet run --urls http://localhost:5000
```
Local HTTP only works in Development. Production will be HTTPS only (URL to be provided).

Real phones cannot reach `localhost`; use the PC's LAN IP and open the firewall port, or ask the backend team for a tunnel.

Flutter local-HTTP settings (debug only, remove for production):
- Android: `android:usesCleartextTraffic="true"` in the debug `AndroidManifest.xml`.
- iOS: add an App Transport Security exception for the dev host in `Info.plist`.

## 2. Client credentials

| Item | Value |
|---|---|
| `client_id` | `FeSekty_Mobile` |
| `client_secret` | **none** (public client – never send or embed a secret) |
| Grant types | `password`, `refresh_token` |
| Scope | `openid profile email roles offline_access FeSekty` |

`offline_access` is required to receive a refresh token.

## 3. Endpoints

| Method | URL | Auth |
|---|---|---|
| GET | `/api/mobile/v1/system/ping` | none |
| POST | `/api/mobile/v1/auth/register` | none |
| POST | `/connect/token` | none (login + refresh) |
| GET | `/api/mobile/v1/profile/me` | `Authorization: Bearer <access_token>` |

Send `Accept-Language: en` (or `ar`) to choose the language of error messages; without it the server may answer in Arabic.

### 3.1 Ping (connectivity check)
```
GET /api/mobile/v1/system/ping
```
`200`: `{"status":"ok","serverTimeUtc":"2026-11-16T16:41:44Z"}`

### 3.2 Register a new end user
```
POST /api/mobile/v1/auth/register
Content-Type: application/json
Accept-Language: en

{
  "userName": "ali",
  "emailAddress": "ali@example.com",
  "password": "Secret123",
  "name": "Ali",
  "surname": "Hassan"
}
```
`name` and `surname` are optional. Rules: `userName`, `emailAddress` (valid email) and `password` are required; password minimum 6 characters (server password policy).

`200` response (the user is created with role `AppUser`; **no tokens are returned** – call login next):
```json
{
  "id": "39036fb3-c8bb-5055-1757-3a24f77347df",
  "userName": "ali",
  "name": "Ali",
  "surname": "Hassan",
  "email": "ali@example.com",
  "roles": ["AppUser"]
}
```

Errors (HTTP 403 or 400, body `{"error":{"message":..., "validationErrors":[...]}}`):
| Case | `error.message` (English) |
|---|---|
| Username taken | `Username 'ali' is already taken.` |
| Password too short | `Passwords must be at least 6 characters.` |
| Invalid email / missing field | `The request is not valid` + `validationErrors[].members` naming the field |

### 3.3 Login
```
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&client_id=FeSekty_Mobile&username=ali&password=Secret123&scope=openid profile email roles offline_access FeSekty
```
`username` may be the user name or the email.

`200`:
```json
{
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 3599,
  "id_token": "eyJ...",
  "refresh_token": "eyJ..."
}
```
Wrong credentials, `400`:
```json
{ "error": "invalid_grant", "error_description": "Invalid username or password!" }
```
Note: this endpoint takes **form-urlencoded**, not JSON, and errors use the OAuth shape (`error`, `error_description`), unlike the `/api/mobile` endpoints.

### 3.4 Refresh the access token
```
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=refresh_token&client_id=FeSekty_Mobile&refresh_token=<refresh_token>
```
Returns a new `access_token` **and a new `refresh_token`** – always replace the stored refresh token with the new one. Invalid/used token: `400 invalid_grant`. When refresh fails, send the user to the login screen.

### 3.5 Current user
```
GET /api/mobile/v1/profile/me
Authorization: Bearer <access_token>
```
`200`: same shape as the register response. `401` with no/expired/invalid token.

## 4. Recommended app flow

1. Register -> (success) -> Login with the same credentials.
2. Store `access_token`, `refresh_token`, and expiry (`now + expires_in`) in `flutter_secure_storage` (not SharedPreferences).
3. Attach `Authorization: Bearer ...` to every API call.
4. On `401`: refresh once, retry the request once; if refresh fails, clear storage and show Login.
5. Logout: delete the stored tokens.

## 5. Flutter example (dio)

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'http://10.0.2.2:5000',   // iOS simulator: http://localhost:5000
  headers: {'Accept-Language': 'en'},
));

Future<void> register(String user, String email, String pass) async {
  await dio.post('/api/mobile/v1/auth/register',
      data: {'userName': user, 'emailAddress': email, 'password': pass});
}

Future<Map<String, dynamic>> login(String user, String pass) async {
  final r = await dio.post('/connect/token',
      data: {
        'grant_type': 'password',
        'client_id': 'FeSekty_Mobile',
        'username': user,
        'password': pass,
        'scope': 'openid profile email roles offline_access FeSekty',
      },
      options: Options(contentType: Headers.formUrlEncodedContentType));
  return r.data;   // access_token, refresh_token, expires_in
}

Future<Map<String, dynamic>> me(String accessToken) async {
  final r = await dio.get('/api/mobile/v1/profile/me',
      options: Options(headers: {'Authorization': 'Bearer $accessToken'}));
  return r.data;
}
```
Handle errors: for `/api/mobile` calls read `e.response?.data['error']['message']`; for `/connect/token` read `error_description`.

## 6. Testing

### 6.1 curl (copy/paste)
```bash
B=http://localhost:5000

# ping
curl $B/api/mobile/v1/system/ping

# register
curl -X POST $B/api/mobile/v1/auth/register -H "Content-Type: application/json" -H "Accept-Language: en" \
  -d '{"userName":"ali","emailAddress":"ali@example.com","password":"Secret123"}'

# login
curl -X POST $B/connect/token \
  -d "grant_type=password&client_id=FeSekty_Mobile&username=ali&password=Secret123&scope=openid profile email roles offline_access FeSekty"

# me (paste the access_token)
curl $B/api/mobile/v1/profile/me -H "Authorization: Bearer <access_token>"
```

### 6.2 Postman
1. Create environment variable `baseUrl = http://localhost:5000`.
2. **Register**: POST `{{baseUrl}}/api/mobile/v1/auth/register`, Body -> raw JSON.
3. **Login**: POST `{{baseUrl}}/connect/token`, Body -> **x-www-form-urlencoded** with the fields from 3.3. In Tests tab: `pm.environment.set("token", pm.response.json().access_token)`.
4. **Me**: GET `{{baseUrl}}/api/mobile/v1/profile/me`, Authorization -> Bearer Token -> `{{token}}`.

### 6.3 Swagger
`http://localhost:5000/swagger` lists the `/api/mobile/v1/...` endpoints and request/response models. It has no Authorize button for the mobile client yet, so call secured endpoints with curl/Postman.

### 6.4 Test checklist
- [ ] Ping returns `ok` from the emulator (proves networking/cleartext setup).
- [ ] Register: new user OK; same username again -> "already taken"; short password -> rejected; bad email -> validation error.
- [ ] Login: valid -> tokens; wrong password -> `invalid_grant`.
- [ ] `/me` with token -> 200; without token -> 401.
- [ ] Refresh returns a new token pair; old refresh token no longer works.
- [ ] After app restart the user is still logged in (tokens persisted).

### 6.5 Test accounts
Use your own registered users. Existing dev accounts: `mobiletest` / `Test@12345`, `weakuser` / `abcdef` (backend team will delete them later). Do not use the `admin` account in the mobile app.

## 7. Known limitations / coming next
- No email confirmation or "forgot password" endpoint yet.
- No logout/revoke endpoint (delete tokens on the device).
- Password grant is used for now; migration to Authorization Code + PKCE is planned.
- Access token lifetime ~1 hour (`expires_in`).
