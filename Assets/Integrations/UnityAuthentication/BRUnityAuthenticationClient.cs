#if BR_UNITY_AUTHENTICATION
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;

// Enable BR_UNITY_AUTHENTICATION after installing Unity Services Core and
// Authentication 2.7.2+ and configuring the provider in the Unity Dashboard.
public class BRUnityAuthenticationClient : MonoBehaviour
{
    [SerializeField] private string verificationEndpoint = "https://YOUR_API_HOST/auth/verify";
    public string PlayerId { get; private set; }
    public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;

    public event Action<string> Authenticated;
    public event Action<string> SignInFailed;

    private bool servicesInitialized;
    private bool operationInProgress;

    public async void SignInAsGuest()
    {
        await RunAuthentication(async () =>
        {
            await EnsureServicesInitialized();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        });
    }

    public async void SignUpWithUsernamePassword(string username, string password)
    {
        await RunAuthentication(async () =>
        {
            await EnsureServicesInitialized();
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
        });
    }

    public async void SignInWithUsernamePassword(string username, string password)
    {
        await RunAuthentication(async () =>
        {
            await EnsureServicesInitialized();
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
        });
    }

    // Call after guest sign-in to make the account recoverable with credentials.
    // Username/password are passed directly to Unity Authentication and never logged or stored here.
    public async void LinkCurrentAccountWithUsernamePassword(string username, string password)
    {
        await RunAuthentication(async () =>
        {
            await EnsureServicesInitialized();
            if (!AuthenticationService.Instance.IsSignedIn)
                throw new InvalidOperationException("Sign in before linking account credentials.");
            await AuthenticationService.Instance.AddUsernamePasswordAsync(username, password);
        });
    }

    private async Task EnsureServicesInitialized()
    {
        if (servicesInitialized) return;
        await UnityServices.InitializeAsync();
        servicesInitialized = true;
    }

    private async Task RunAuthentication(Func<Task> signInOperation)
    {
        if (operationInProgress)
        {
            SignInFailed?.Invoke("A sign-in request is already running.");
            return;
        }

        operationInProgress = true;
        try
        {
            await signInOperation();
            if (!AuthenticationService.Instance.IsSignedIn)
                throw new InvalidOperationException("Authentication did not establish a signed-in session.");

            PlayerId = AuthenticationService.Instance.PlayerId;
            if (string.IsNullOrWhiteSpace(PlayerId) ||
                string.IsNullOrWhiteSpace(AuthenticationService.Instance.AccessToken))
                throw new InvalidOperationException("Authentication returned incomplete credentials.");

            // Do not expose the access token to UI listeners. Verify it directly with the backend.
            StartCoroutine(VerifyBackendSession(AuthenticationService.Instance.AccessToken));
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Unity Authentication failed: " + exception.GetType().Name);
            SignInFailed?.Invoke("Sign-in failed. Check your details and try again.");
        }
        finally
        {
            operationInProgress = false;
        }
    }

    private IEnumerator VerifyBackendSession(string accessToken)
    {
        if (!IsSafeVerificationEndpoint(verificationEndpoint))
        {
            SignInFailed?.Invoke("Set a valid HTTPS authentication server URL in the Inspector.");
            yield break;
        }

        using (UnityWebRequest request = new UnityWebRequest(verificationEndpoint, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(new byte[0]);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 15;
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Backend identity verification failed: HTTP " + request.responseCode);
                SignInFailed?.Invoke("The game server could not verify your account. Please try again.");
                yield break;
            }

            AuthVerificationResponse response = null;
            try { response = JsonUtility.FromJson<AuthVerificationResponse>(request.downloadHandler.text); }
            catch (Exception) { }

            if (response == null || !response.authenticated || response.player == null ||
                string.IsNullOrWhiteSpace(response.player.playerId) ||
                response.player.playerId != AuthenticationService.Instance.PlayerId)
            {
                SignInFailed?.Invoke("The game server returned an invalid account response.");
                yield break;
            }

            PlayerId = response.player.playerId;
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

    public void SignOut()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut(true);
        PlayerId = null;
    }

    [Serializable]
    private class AuthVerificationResponse
    {
        public bool authenticated;
        public PlayerProfile player;
    }

    [Serializable]
    private class PlayerProfile
    {
        public string playerId;
        public string createdAt;
        public string lastSeenAt;
    }
}
#endif
