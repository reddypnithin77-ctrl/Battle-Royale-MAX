#if BR_FIREBASE_AUTHENTICATION
using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Networking;

// Firebase email/password authentication plus server-side ID-token verification.
// Import FirebaseAuth.unitypackage and FirebaseApp dependencies before enabling
// BR_FIREBASE_AUTHENTICATION in Player Settings.
public class BRFirebaseAuthenticationClient : MonoBehaviour
{
    [SerializeField] private string verificationEndpoint = "https://YOUR_API_HOST/auth/firebase/verify";

    public string PlayerId { get; private set; }
    public bool IsSignedIn => auth != null && auth.CurrentUser != null;

    public event Action<string> Authenticated;
    public event Action<string> SignInFailed;

    private FirebaseAuth auth;
    private bool firebaseReady;
    private bool operationInProgress;
    private bool verificationInProgress;

    public async void SignUp(string email, string password)
    {
        await RunAuthOperation(async () =>
        {
            await EnsureFirebaseReady();
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email.Trim(), password);
            await VerifyCurrentUser(result.User);
        });
    }

    public async void SignIn(string email, string password)
    {
        await RunAuthOperation(async () =>
        {
            await EnsureFirebaseReady();
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email.Trim(), password);
            await VerifyCurrentUser(result.User);
        });
    }

    public async void SendPasswordReset(string email)
    {
        await RunAuthOperation(async () =>
        {
            await EnsureFirebaseReady();
            await auth.SendPasswordResetEmailAsync(email.Trim());
            Debug.Log("Firebase password reset email requested.");
        }, false);
    }

    public void SignOut()
    {
        if (auth != null) auth.SignOut();
        PlayerId = null;
    }

    private async Task EnsureFirebaseReady()
    {
        if (firebaseReady) return;
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
            throw new InvalidOperationException("Firebase dependencies are unavailable: " + status);
        auth = FirebaseAuth.DefaultInstance;
        firebaseReady = true;
    }

    private async Task RunAuthOperation(Func<Task> operation, bool verifySession = true)
    {
        if (operationInProgress || verificationInProgress)
        {
            SignInFailed?.Invoke("Another account request is already running.");
            return;
        }

        operationInProgress = true;
        try
        {
            await operation();
        }
        catch (Exception exception)
        {
            // Avoid logging passwords, tokens, or raw provider responses.
            Debug.LogWarning("Firebase account operation failed: " + exception.GetType().Name);
            SignInFailed?.Invoke("Account request failed. Check your details and try again.");
        }
        finally
        {
            operationInProgress = false;
        }
    }

    private async Task VerifyCurrentUser(FirebaseUser user)
    {
        if (user == null) throw new InvalidOperationException("Firebase returned no user.");
        string idToken = await user.TokenAsync(false);
        if (string.IsNullOrWhiteSpace(idToken))
            throw new InvalidOperationException("Firebase returned no ID token.");

        // StartCoroutine runs on Unity's main thread; token is never exposed to UI events.
        StartCoroutine(VerifyBackendSession(idToken, user.UserId));
    }

    private IEnumerator VerifyBackendSession(string idToken, string expectedUid)
    {
        verificationInProgress = true;
        if (!IsSafeVerificationEndpoint(verificationEndpoint))
        {
            SignInFailed?.Invoke("Set a valid HTTPS verification endpoint in the Inspector.");
            verificationInProgress = false;
            yield break;
        }

        using (UnityWebRequest request = new UnityWebRequest(verificationEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(new byte[0]);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 15;
            request.SetRequestHeader("Authorization", "Bearer " + idToken);
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Firebase backend verification failed: HTTP " + request.responseCode);
                SignInFailed?.Invoke("The game server could not verify your account. Try again.");
                verificationInProgress = false;
                yield break;
            }

            FirebaseVerificationResponse response = null;
            try { response = JsonUtility.FromJson<FirebaseVerificationResponse>(request.downloadHandler.text); }
            catch (Exception) { }

            if (response == null || !response.authenticated || response.player == null ||
                string.IsNullOrWhiteSpace(response.player.playerId) ||
                response.player.playerId != expectedUid)
            {
                SignInFailed?.Invoke("The game server returned an invalid account response.");
                verificationInProgress = false;
                yield break;
            }

            PlayerId = response.player.playerId;
            verificationInProgress = false;
            Authenticated?.Invoke(PlayerId);
        }
    }

    private static bool IsSafeVerificationEndpoint(string endpoint)
    {
        Uri uri;
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out uri)) return false;
        if (uri.Scheme == Uri.UriSchemeHttps) return true;
        return Application.isEditor && uri.Scheme == Uri.UriSchemeHttp &&
            (uri.Host == "localhost" || uri.Host == "127.0.0.1");
    }

    [Serializable]
    private class FirebaseVerificationResponse
    {
        public bool authenticated;
        public FirebasePlayerProfile player;
    }

    [Serializable]
    private class FirebasePlayerProfile
    {
        public string playerId;
        public string email;
        public string createdAt;
        public string lastSeenAt;
    }
}
#endif
