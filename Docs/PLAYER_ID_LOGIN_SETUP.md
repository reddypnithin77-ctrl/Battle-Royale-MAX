# Firebase Authentication setup

The game now uses Firebase Authentication email/password accounts. The old Unity Authentication adapter remains in the repository for reference but is not used by this Firebase flow.

## 1. Firebase Console

1. Open https://console.firebase.google.com/ and select your Firebase project.
2. Open **Authentication → Sign-in method** (or **Sign-in providers**).
3. Enable **Email/Password**, then save.
4. In **Project settings → General**, register an Android app using the exact Android application ID from Unity Player Settings.
5. Download `google-services.json` and place it directly inside the Unity project's `Assets/` folder.
6. Download the Firebase Unity SDK from the official setup guide and import `FirebaseAuth.unitypackage` into Unity. Import the SDK's required dependency resolver if prompted.

Official setup: https://firebase.google.com/docs/unity/setup
Email/password guide: https://firebase.google.com/docs/auth/unity/password-auth

## 2. Unity client

1. Add `BR_FIREBASE_AUTHENTICATION` to Player Settings → Other Settings → Scripting Define Symbols.
2. Add `BRFirebaseAuthenticationClient` to a scene object.
3. Set Verification Endpoint to your deployed HTTPS URL ending in `/auth/firebase/verify`.
4. Wire your Sign Up UI to `SignUp(email, password)`, Sign In UI to `SignIn(email, password)`, and Forgot Password UI to `SendPasswordReset(email)`.
5. Subscribe to `Authenticated` and `SignInFailed`. Enter the game only after the `Authenticated` event fires.

Firebase Auth in this implementation uses email/password, not a separate username. Never log or store passwords or ID tokens in the client.

## 3. Backend identity verification

The backend verifies Firebase ID tokens with Firebase Admin SDK and derives the player ID from the verified UID. It stores a basic player profile in PostgreSQL. The client cannot choose its player ID.

Required environment variables:
- `FIREBASE_PROJECT_ID`: exact Firebase project ID.
- `DATABASE_URL`: private PostgreSQL connection string.
- `GOOGLE_APPLICATION_CREDENTIALS`: path to a Firebase service-account credential file for local development only.

For local Docker:
1. Create a Firebase service-account credential for server-side use in Firebase/Google Cloud project settings. Keep it private.
2. Put the JSON file in a local `secrets/` folder, which is ignored by Git; never upload it to GitHub or include it in the APK.
3. Copy `.env.example` to `.env`, set the Firebase project ID, service-account file path, and a strong local database password.
4. Run `docker compose up --build`.
5. Check `http://127.0.0.1:8080/health`.
6. Stop with `docker compose down`.

Production must use a managed secret mechanism or workload identity instead of a downloaded service-account key, HTTPS ingress, managed PostgreSQL with TLS/backups, distributed rate limiting, monitoring, and restricted database access. The included Compose setup is local development only.

## 4. Important limitations

- Firebase SDK import, Unity scene wiring, and Android build still require the Unity Editor and your local project.
- Firebase service-account credentials and database credentials must never be placed in the client, APK, or Git repository.
- This is an authentication/profile foundation, not a full multiplayer backend. It does not implement matchmaking, persistent inventory, passwordless recovery support, moderation, or authoritative gameplay.
- The previous Unity Authentication verifier endpoint has been replaced by the Firebase endpoint. Do not use the old `BRUnityAuthenticationClient` for this flow.
