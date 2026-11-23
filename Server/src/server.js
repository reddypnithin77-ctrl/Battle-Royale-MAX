import { createServer } from "node:http";
import { applicationDefault, initializeApp } from "firebase-admin/app";
import { getAuth } from "firebase-admin/auth";
import pg from "pg";
import { createIpRateLimiter, isValidFirebaseClaims } from "./auth-security.js";

const { Pool } = pg;
const PORT = Number(process.env.PORT || 8080);
const FIREBASE_PROJECT_ID = process.env.FIREBASE_PROJECT_ID;
const DATABASE_URL = process.env.DATABASE_URL;

if (!FIREBASE_PROJECT_ID) throw new Error("Set FIREBASE_PROJECT_ID.");
if (!DATABASE_URL) throw new Error("Set DATABASE_URL to a private PostgreSQL database.");

initializeApp({
  credential: applicationDefault(),
  projectId: FIREBASE_PROJECT_ID,
});
const firebaseAuth = getAuth();
const pool = new Pool({
  connectionString: DATABASE_URL,
  ssl: process.env.PGSSLMODE === "disable" ? false : { rejectUnauthorized: true },
  max: 10,
  connectionTimeoutMillis: 5000,
  idleTimeoutMillis: 30000,
});
const isRateLimited = createIpRateLimiter({ limit: 30, windowMs: 60_000, maxEntries: 10_000 });

await pool.query(`
  CREATE TABLE IF NOT EXISTS player_profiles (
    player_id TEXT PRIMARY KEY,
    email TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_seen_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
  )
`);

function sendJson(response, status, body) {
  response.writeHead(status, {
    "Content-Type": "application/json; charset=utf-8",
    "Cache-Control": "no-store",
    "X-Content-Type-Options": "nosniff",
  });
  response.end(JSON.stringify(body));
}

const server = createServer(async (request, response) => {
  if (request.method === "GET" && request.url === "/health") {
    try {
      await pool.query("SELECT 1");
      return sendJson(response, 200, { ok: true, database: "connected" });
    } catch {
      return sendJson(response, 503, { ok: false, database: "unavailable" });
    }
  }

  if (request.method !== "POST" || request.url !== "/auth/firebase/verify") {
    return sendJson(response, 404, { error: "not_found" });
  }

  const clientIp = request.socket.remoteAddress || "unknown";
  if (isRateLimited(clientIp)) return sendJson(response, 429, { error: "rate_limited" });

  const authorization = request.headers.authorization || "";
  const match = /^Bearer ([^\s]+)$/.exec(authorization);
  if (!match) return sendJson(response, 401, { authenticated: false, error: "missing_token" });

  try {
    // Firebase Admin verifies signature, expiry, issuer, audience and revocation.
    const decoded = await firebaseAuth.verifyIdToken(match[1], true);
    if (!isValidFirebaseClaims(decoded, FIREBASE_PROJECT_ID)) {
      return sendJson(response, 401, { authenticated: false, error: "invalid_identity" });
    }

    const result = await pool.query(
      `INSERT INTO player_profiles (player_id, email)
       VALUES ($1, $2)
       ON CONFLICT (player_id)
       DO UPDATE SET email = EXCLUDED.email, last_seen_at = NOW()
       RETURNING player_id AS "playerId", email,
                 created_at AS "createdAt", last_seen_at AS "lastSeenAt"`,
      [decoded.uid, typeof decoded.email === "string" ? decoded.email : null]
    );

    return sendJson(response, 200, { authenticated: true, player: result.rows[0] });
  } catch (error) {
    // Do not expose verifier errors or token contents to clients.
    const code = error && typeof error.code === "string" ? error.code : "";
    const status = code.startsWith("auth/") ? 401 : 503;
    return sendJson(response, status, {
      authenticated: false,
      error: status === 401 ? "invalid_token" : "verification_unavailable",
    });
  }
});

server.listen(PORT, "0.0.0.0", () => {
  console.log(`Firebase auth verifier listening on port ${PORT}`);
});

async function shutdown() {
  server.close();
  await pool.end();
}
process.on("SIGTERM", shutdown);
process.on("SIGINT", shutdown);
