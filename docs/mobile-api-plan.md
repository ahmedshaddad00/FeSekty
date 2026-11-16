# FeSekty – Mobile (Flutter) API plan

## 0. What the solution already has (verified in the code)

| Area | Current state |
|---|---|
| Hosting | One host, `FeSekty.Web`, is the MVC UI **and** the OpenIddict auth server **and** the API (no separate HttpApi.Host). |
| API exposure | `FeSektyWebModule.ConfigureAutoApiControllers` exposes every `ApplicationService` in `FeSekty.Application` as `/api/app/...`. |
| Swagger | Enabled (`/swagger`), client `FeSekty_Swagger` (auth-code flow). |
| Auth | OpenIddict + `AspNetCore.Authentication.JwtBearer` package present. Client `FeSekty_App` is seeded with Password, AuthorizationCode, ClientCredentials, RefreshToken grants (`OpenIddictDataSeedContributor`), but `appsettings.json` of DbMigrator gives it **no RootUrl/secret** -> not usable by a mobile app yet. |
| Dev URL | `http://localhost:5000` (when started without launch profile) / `https://localhost:44343` (launch profile). |
| DB | SQL Server `.\SHADDAD`, database `FeSekty`. |

Key consequence: **no new backend project is required**. The mobile API is a set of application services + a dedicated OpenIddict client, in the existing layers.

---

## 1. Phase A – API layer foundation (do once)

Goal: a stable, versioned, documented API surface the Flutter app can rely on.

1. **Contracts first** – in `FeSekty.Application.Contracts` create a `Mobile` folder:
   `IXxxMobileAppService` interfaces + DTOs. Keep mobile DTOs separate from web/admin ones so the UI can evolve independently.
2. **Implementation** – in `FeSekty.Application/Mobile`, classes deriving from `FeSektyAppService`. Business logic stays in Domain; app services only orchestrate.
3. **Route convention** – use `[RemoteService]` + `[Route("api/mobile/...")]` or `ConventionalControllers` root path `mobile` so all mobile endpoints are under `/api/mobile/v1/...` (separate from `/api/app`). Add API versioning (`Asp.Versioning`, ABP supports `AddAbpApiVersioning`) from day one.
4. **Permissions** – define in `FeSektyPermissions`; every mobile endpoint is `[Authorize]` except login-related ones. Create an `AppUser` role (already started in the UI) for end users and grant only the mobile permissions to it.
5. **Cross-cutting**
   - Error shape: rely on ABP's `RemoteServiceErrorResponse` (stable JSON for Flutter); throw `UserFriendlyException` / `BusinessException` with codes.
   - CORS: only needed for Flutter **web**; native Android/iOS don't need it. Add `App:CorsOrigins` if web is targeted.
   - HTTPS: Android emulator reaches host at `http://10.0.2.2:5000`; real devices need the PC's LAN IP + firewall rule, or a dev tunnel. Production must be HTTPS only.
6. **Swagger** – add a "Mobile" Swagger doc group + a bearer-token security definition so endpoints can be tested with a token.
7. **Client generation (optional)** – export `swagger.json` and generate a Dart client with `openapi-generator` (`dart-dio`) so DTOs aren't hand-written in Flutter.
8. **Tests** – `FeSekty.Application.Tests` for each mobile app service (ABP test base already exists).

Deliverable: `GET /api/mobile/v1/ping` (anonymous) and `GET /api/mobile/v1/me` (authorized) visible in Swagger.

---

## 2. Phase B – First API: end-user login from Flutter

### Decision needed: login mechanism

| Option | How | Pros | Cons |
|---|---|---|---|
| **1. OAuth password grant on `/connect/token`** (recommended for step 1) | Flutter posts username+password to the existing token endpoint with a dedicated public client `FeSekty_Mobile`. | Zero custom auth code, returns standard access + refresh tokens, works with ABP `[Authorize]` immediately. | Native-UI login form handles raw passwords in the app; password grant is discouraged by OAuth 2.1. Fine for first-party app. |
| 2. Authorization Code + PKCE (`flutter_appauth`) | App opens the ABP login page in a system browser. | Most secure, supports external logins/2FA for free. | Needs redirect scheme config; UI is the web login page, not Flutter-native. |
| 3. Custom `POST /api/mobile/v1/auth/login` app service | Validate with `SignInManager`/`IdentityUserManager`, issue token yourself. | Full control of response shape. | You re-implement token issuing/refresh/lockout; high security risk. **Not recommended.** |

Plan below = Option 1 now, with Option 2 as the hardening step.

### Backend steps
1. **Add OpenIddict client** `FeSekty_Mobile` in `OpenIddictDataSeedContributor` (public client, grants: `password`, `refresh_token`; scopes: `FeSekty`, `offline_access`, `openid`, `profile`, `email`, `roles`). Config keys in `DbMigrator/appsettings.json` and `Web/appsettings.json`.
2. Re-run `FeSekty.DbMigrator` to seed it.
3. Token lifetimes: set access token ~15-60 min, refresh token ~30 days (`OpenIddictServerOptions`), refresh token rotation on.
4. **Account lockout / password policy**: confirm `Abp.Identity.Lockout.*` settings (max failed attempts) – the token endpoint honors them.
5. **Add `GET /api/mobile/v1/me`** returning `{ id, userName, name, surname, email, roles }` from `ICurrentUser` – this is the "am I logged in?" check and the first real API.
6. **Optional:** `POST /api/mobile/v1/auth/register` (self-registration, `AllowSelfRegistration` setting) assigning the `AppUser` role; and `POST /api/account/send-password-reset-code` (already provided by ABP Account module) for forgot-password.
7. Test with Swagger/curl:
   ```
   POST /connect/token
   grant_type=password&client_id=FeSekty_Mobile&username=...&password=...&scope=FeSekty offline_access
   ```
   then `GET /api/mobile/v1/me` with `Authorization: Bearer <token>`.

### Flutter steps
1. Packages: `dio`, `flutter_secure_storage`, (optional) `flutter_appauth`, `riverpod`/`bloc`.
2. `AuthRepository.login(user, pass)` -> POST form-urlencoded to `/connect/token`; store `access_token`, `refresh_token`, expiry in `flutter_secure_storage` (never SharedPreferences).
3. Dio **interceptor**: attach bearer token; on 401 call refresh (`grant_type=refresh_token`) once, retry, else route to login.
4. Call `/api/mobile/v1/me` after login to load profile; handle ABP error JSON (`error.message`).
5. Logout: delete stored tokens (+ optionally revoke via `/connect/revocation`).
6. Base URL config per flavor: dev `http://10.0.2.2:5000` (Android emulator), staging/prod HTTPS.

### Done when
- Valid credentials return tokens; wrong credentials show a friendly message; locked-out account is handled.
- `/me` works with the token and 401s without it.
- Token refresh works after access-token expiry.
- Tokens persist across app restarts.

---

## 3. Order of work / estimates

| # | Task | Est. |
|---|---|---|
| 1 | Phase A items 1-3, 6 + ping endpoint | 0.5 day |
| 2 | Seed `FeSekty_Mobile`, token lifetimes, test via curl | 0.5 day |
| 3 | `/me` endpoint + permissions + tests | 0.5 day |
| 4 | Flutter auth repo, secure storage, Dio interceptor, login screen | 1-1.5 days |
| 5 | Refresh flow + error handling + logout | 0.5 day |
| 6 | Hardening: PKCE migration, rate limiting, HTTPS/prod config | 1 day |

## 4. Open questions for you
1. Which login method: password grant now (Option 1) or go straight to PKCE (Option 2)?
2. Is end-user self-registration needed, or are users created by admins?
3. Which platforms: Android only, Android + iOS, Flutter web?
4. Where will it be hosted (so we can plan the HTTPS URL and CORS)?

---

## 5. Implemented (decisions: password grant, self-registration, Android+iOS, localhost/emulator)

| Endpoint | Auth | Purpose |
|---|---|---|
| `GET  /api/mobile/v1/system/ping` | anonymous | connectivity check |
| `POST /api/mobile/v1/auth/register` | anonymous | self-register; user gets role `AppUser` |
| `POST /connect/token` | client `FeSekty_Mobile` | login (`grant_type=password`) and refresh (`grant_type=refresh_token`) |
| `GET  /api/mobile/v1/profile/me` | Bearer token | current user; 401 without token |

Code: `src/FeSekty.Application.Contracts/Mobile`, `src/FeSekty.Application/Mobile`, client seed in `OpenIddictDataSeedContributor`, routes in `FeSektyWebModule.ConfigureAutoApiControllers`.

Run for the emulator (HTTP, Development): `ASPNETCORE_ENVIRONMENT=Development dotnet run --urls http://localhost:5000` in `src/FeSekty.Web`
(`appsettings.Development.json` sets `AuthServer:RequireHttpsMetadata=false`, which lets `/connect/token` accept HTTP).

Base URLs from Flutter: Android emulator `http://10.0.2.2:5000`, iOS simulator `http://localhost:5000`.
Android needs `android:usesCleartextTraffic="true"` (debug manifest) and iOS an ATS exception for local HTTP; remove both for production (HTTPS only).

Login request (form-urlencoded):
`grant_type=password&client_id=FeSekty_Mobile&username=..&password=..&scope=openid profile email roles offline_access FeSekty`
