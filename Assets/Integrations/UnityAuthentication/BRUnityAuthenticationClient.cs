#if BR_UNITY_AUTHENTICATION
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

// Enable BR_UNITY_AUTHENTICATION only after installing Unity Services Core
// and Authentication packages through Package Manager.
public class BRUnityAuthenticationClient : MonoBehaviour
{
    public string PlayerId { get; private set; }
    public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;

    public event Action<string, string> Authenticated;
    public event Action<string> SignInFailed;

    public async void SignInAsGuest()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            PlayerId = AuthenticationService.Instance.PlayerId;
            string accessToken = AuthenticationService.Instance.AccessToken;
            if (string.IsNullOrWhiteSpace(PlayerId) || string.IsNullOrWhiteSpace(accessToken))
                throw new InvalidOperationException("Authentication returned an empty identity or token.");

            // Send this token over HTTPS to the game's backend.
            // The backend must verify its signature and claims, then derive the player ID from 'sub'.
            Authenticated?.Invoke(PlayerId, accessToken);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Unity Authentication sign-in failed: " + exception.GetType().Name);
            SignInFailed?.Invoke("Sign-in failed. Please try again.");
        }
    }

    public void SignOut()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            AuthenticationService.Instance.SignOut();
        PlayerId = null;
    }
}
#endif
