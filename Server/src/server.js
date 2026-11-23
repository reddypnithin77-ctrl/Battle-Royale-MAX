import { createServer } from "node:http";
import { createRemoteJWKSet, jwtVerify } from "jose";

const PORT = Number(process.env.PORT || 8080);
const EXPECTED_PROJECT_ID = process.env.UNITY_PROJECT_ID;
const ISSUER = "https://player-auth.services.api.unity.com";
const JWKS = createRemoteJWKSet(
  new URL("https://player-auth.services.api.unity.com/.well-known/jwks.json")
);

if (!EXPECTED_PROJECT_ID) {
  throw new Error("Set UNITY_PROJECT_ID in the server environment before starting.");
}

const server = createServer(async (request, response) => {
  response.setHeader("Content-Type", "application/json; charset=utf-8");
  response.setHeader("Cache-Control", "no-store");

  if (request.method === "GET" && request.url === "/health") {
    response.writeHead(200);
    response.end(JSON.stringify({ ok: true }));
    return;
  }

  if (request.method !== "POST" || request.url !== "/auth/verify") {
    response.writeHead(404);
    response.end(JSON.stringify({ error: "not_found" }));
    return;
  }

  const authorization = request.headers.authorization || "";
  const match = /^Bearer ([A-Za-z0-9._~-]+)$/.exec(authorization);
  if (!match) {
    response.writeHead(401);
    response.end(JSON.stringify({ error: "missing_bearer_token" }));
    return;
  }

  try {
    const { payload } = await jwtVerify(match[1], JWKS, {
      issuer: ISSUER,
      algorithms: ["RS256"],
    });

    if (typeof payload.sub !== "string" || payload.sub.length === 0 ||
        payload.project_id !== EXPECTED_PROJECT_ID) {
      response.writeHead(401);
      response.end(JSON.stringify({ error: "invalid_identity_claims" }));
      return;
    }

    // Never accept a player ID from the request body. The verified 'sub' claim is authoritative.
    response.writeHead(200);
    response.end(JSON.stringify({ authenticated: true, playerId: payload.sub }));
  } catch {
    response.writeHead(401);
    response.end(JSON.stringify({ error: "invalid_or_expired_token" }));
  }
});

server.listen(PORT, "0.0.0.0", () => {
  process.stdout.write(`Auth verification service listening on port ${PORT}\n`);
});
